namespace Lazy;

public class LazyMultithreading<T> : ILazy<T>
{
    public LazyMultithreading(Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        supplier = func;
    }
    
    private Object lockObject = new();

    private Func<T> supplier;

    private T? result;

    private volatile bool isComputed;

    private volatile Exception? supplierException;
    
    public T? Get()
    {
        if (supplierException is null)
        {
            throw supplierException;
        }
        lock (lockObject)
        {
            if (!isComputed)
            {
                try
                {
                    result = supplier();
                }
                catch (Exception e)
                {
                    supplierException = e;
                    throw;
                }
                finally
                {
                    supplier = null;
                    isComputed = true;
                }
            }
            return result;
        }
    }
}