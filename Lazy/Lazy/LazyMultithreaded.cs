namespace Lazy;

public class LazyMultithreading<T> : ILazy<T>
{
    public LazyMultithreading(Func<T> func)
    {
        supplier = func;
    }
    
    private Object lockObject = new();

    private Func<T> supplier;

    private T? result;

    private volatile bool isComputed;
    
    public T? Get()
    {
        lock (lockObject)
        {
            if (!isComputed)
            {
                result = supplier();
                isComputed = true;
            }
            Monitor.Pulse(lockObject);
            return result;
        }
    }
}