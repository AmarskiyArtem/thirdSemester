namespace ThreadPool;

using System.Collections.Concurrent;

/// <summary>
/// Class that represents Thread Pool.
/// </summary>
public class MyThreadPool
{
    private readonly Thread[] _threads;
    private readonly CancellationTokenSource _cts = new();
    private readonly BlockingCollection<Action> _tasks = new();
    private readonly Mutex _mutex = new();
    
    /// <summary>
    /// Thread Pool constructor
    /// </summary>
    /// <param name="threadCount">Number of thread</param>
    public MyThreadPool(int threadCount)
    {
        if (threadCount < 2)
        {
            throw new ArgumentException("Minimum number of threads: 2");
        }

        _threads = new Thread[threadCount];
        InitThreads();
    }

    private void InitThreads()
    {
        for (var i = 0; i < _threads.Length; i++)
        {
            _threads[i] = new(() =>
            {
                foreach (var task in _tasks.GetConsumingEnumerable())
                {
                    task();
                }
            });
            _threads[i].Start();
        }
    }
    
    /// <summary>
    /// Adds task to pool.
    /// </summary>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (_cts.IsCancellationRequested)
        {
            throw new InvalidOperationException("Thread pool is already shut downed!");
        }
        
        _mutex.WaitOne();
        
        var task = new MyTask<TResult>(function, this);
        _tasks.Add(task.ComputeResult);

        _mutex.ReleaseMutex();
        
        return task;
    }
    
    /// <summary>
    /// Shuts down the thread pool.
    /// </summary>
    public void Shutdown()
    {
        _mutex.WaitOne();
        
        _cts.Cancel();
        _tasks.CompleteAdding();
        
        _mutex.ReleaseMutex();
        
        foreach (var thread in _threads)
        {
            thread.Join();
        }
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private volatile bool _isCompleted;
        private TResult? _result;
        private readonly Func<TResult> _taskFunction;
        private Exception? _taskFuncException;
        private readonly MyThreadPool _threadPool;
        private readonly object _taskLockObject = new();
        private readonly ManualResetEvent _resultIsCompletedEvent = new(false);
        private readonly ConcurrentQueue<Action> _continuationTasks = new();
        
        public MyTask(Func<TResult> taskFunction, MyThreadPool threadPool)
        {
            this._taskFunction = taskFunction;
            this._threadPool = threadPool;
            _isCompleted = false;
        }

        /// <inheritdoc/>
        public bool IsCompleted => _isCompleted;

        /// <inheritdoc/>
        public TResult? Result
        {
            get
            {
                if (!_isCompleted)
                {
                    _resultIsCompletedEvent.WaitOne();
                }

                if (_taskFuncException != null)
                {
                    throw new AggregateException(_taskFuncException);
                }

                return _result;
            }
        }
        
        public void ComputeResult()
        {
            lock (_taskLockObject)
            {
                try
                {
                    _result = _taskFunction();
                }
                catch (Exception e)
                {
                    _taskFuncException = e;
                }
                finally
                {
                    _isCompleted = true;
                    _resultIsCompletedEvent.Set();

                    foreach (var task in _continuationTasks)
                    {
                        _threadPool._tasks.Add(task);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function)
        {
            if (_threadPool._cts.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }

            lock (_taskLockObject)
            {
                if (_isCompleted)
                {
                    return _threadPool.Submit(() => function(Result));
                }
                var continuation = new MyTask<TNewResult>(() => function(Result), _threadPool);
                _continuationTasks.Enqueue(continuation.ComputeResult);
                return continuation;
            }
        }
    }
}