namespace ThreadPool;

using System.Collections.Concurrent;

public class MyThreadPool
{
    private readonly Thread[] _threads;
    private CancellationTokenSource _cts = new();
    private BlockingCollection<Action> tasks = new();
    private object _lockObject = new();
    
    
    public MyThreadPool(int threadCount)
    {
        if (threadCount < 2)
        {
            throw new ArgumentException("Minimum number of threads: two");
        }

        _threads = new Thread[threadCount];
    }
    
    private void InitThreads()
    {
        for (var i = 0; i < _threads.Length; i++)
        {
            _threads[i] = new(() =>
            {
                foreach (var task in tasks.GetConsumingEnumerable())
                {
                    task();
                }
            });
            _threads[i].Start();
        }
    }
    
    public int ThreadCount { get;}
}