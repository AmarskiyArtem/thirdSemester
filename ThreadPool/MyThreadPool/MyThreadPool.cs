namespace ThreadPool;

using System.Threading;

public class MyThreadPool
{
    public int ThreadCount { get;}
    public MyThreadPool(int threadCount)
    {
        ThreadCount = threadCount;
    }
}