namespace ThreadPool;

using System.Collections.Concurrent;


/// <summary>
/// Class that represents Thread Pool instances.
/// </summary>
public class MyThreadPool
{
    private readonly Thread[] threads;
    private CancellationTokenSource cancellationTokenSource;
    private BlockingCollection<Action> tasks;
    private object lockObject;
    private readonly Mutex _mutex = new(false);

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="threadAmount">Amount of threads that will be made in thread pool.</param>
    public MyThreadPool(int threadAmount)
    {
        if (threadAmount < 1)
        {
            throw new ArgumentException("Amount of threads can't be negative!");
        }

        threads = new Thread[threadAmount];
        cancellationTokenSource = new CancellationTokenSource();
        tasks = new BlockingCollection<Action>();
        lockObject = new();

        InitThreads();
    }

    private void InitThreads()
    {
        for (var i = 0; i < threads.Length; i++)
        {
            threads[i] = new(() =>
            {
                foreach (var task in tasks.GetConsumingEnumerable())
                {
                    task();
                }
            });
            threads[i].Start();
        }
    }

    /// <summary>
    /// Adds a new task to the pool.
    /// </summary>
    /// <param name="function">Task's function.</param>
    /// <typeparam name="TResult">Function's return type.</typeparam>
    /// <returns>Made task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> function)
    {
        if (cancellationTokenSource.IsCancellationRequested)
        {
            throw new InvalidOperationException("Thread pool is already shut downed!");
        }
        
        _mutex.WaitOne();
        
        var task = new MyTask<TResult>(function, this);
        tasks.Add(task.ComputeResult);
        
        _mutex.ReleaseMutex();
        
        return task;
    }

    /// <summary>
    /// Shuts down the thread pool.
    /// </summary>
    public void Shutdown()
    {
        _mutex.WaitOne();
        
        cancellationTokenSource.Cancel();
        tasks.CompleteAdding();
        
        _mutex.ReleaseMutex();
        
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    private class MyTask<TResult> : IMyTask<TResult>
    {
        private volatile bool isCompleted;
        private TResult? result;
        private readonly Func<TResult> taskFunction;
        private Exception? taskFuncException;
        private readonly MyThreadPool threadPool;
        private readonly object taskLockObject;
        private readonly ManualResetEvent resultIsCompletedEvent;
        private ConcurrentQueue<Action> continuationTasks;
        private readonly Mutex _taskMutex = new(false);

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="taskFunction">Task's function.</param>
        /// <param name="threadPool">Thread pool where task was added.</param>
        public MyTask(Func<TResult> taskFunction, MyThreadPool threadPool)
        {
            this.taskFunction = taskFunction;
            this.threadPool = threadPool;
            isCompleted = false;
            taskLockObject = new object();
            resultIsCompletedEvent = new ManualResetEvent(false);
            continuationTasks = new ConcurrentQueue<Action>();
        }

        /// <inheritdoc/>
        public bool IsCompleted => isCompleted;

        /// <inheritdoc/>
        public TResult? Result
        {
            get
            {
                if (!isCompleted)
                {
                    resultIsCompletedEvent.WaitOne();
                }

                if (taskFuncException != null)
                {
                    throw new AggregateException(taskFuncException);
                }

                return result;
            }
        }

        /// <summary>
        /// Computes the result of the task's function.
        /// </summary>
        public void ComputeResult()
        {
            _taskMutex.WaitOne();
            
            try
            {
                result = taskFunction();
            }
            catch (Exception e)
            {
                taskFuncException = e;
            }
            finally
            {
                isCompleted = true;
                resultIsCompletedEvent.Set();

                foreach (var task in continuationTasks)
                {
                    threadPool.tasks.Add(task);
                }
                _taskMutex.ReleaseMutex();
            }
        }

        /// <inheritdoc/>
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> function)
        {
            if (threadPool.cancellationTokenSource.IsCancellationRequested)
            {
                throw new InvalidOperationException();
            }
            _taskMutex.WaitOne();
            if (isCompleted)
            {
                _taskMutex.ReleaseMutex();
                return threadPool.Submit(() => function(Result));
            }
            var continuation = new MyTask<TNewResult>(() => function(Result), threadPool);
            continuationTasks.Enqueue(continuation.ComputeResult);
                
            
            return continuation;
        }
    }
}