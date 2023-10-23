namespace ThreadPool;

using System.Threading;

public class MyThreadPool
{
    private readonly Queue<Action> _tasks;
    private readonly 
    
    public int ThreadCount { get;}
    public MyThreadPool(int threadCount)
    {
        ThreadCount = threadCount;
    }
}