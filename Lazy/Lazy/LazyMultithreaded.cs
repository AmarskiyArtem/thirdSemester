namespace Lazy;

public class LazyMultithreading<T> : ILazy<T>
{
    public LazyMultithreading(Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        _supplier = func;
    }

    private readonly Mutex _mutex = new();

    private Func<T>? _supplier;

    private T? _result;

    private volatile bool _isComputed;

    private volatile Exception? _supplierException;
    
    public T? Get()
    {
        if (_supplierException is not null)
        {
            throw _supplierException;
        }

        if (_isComputed)
        {
            return _result;
        }
        
        _mutex.WaitOne();
        if (_isComputed)
        {
            return Get();
        }

        try
        {
            _result = _supplier!();
        }
        catch (Exception e)
        {
            _supplierException = e;
            throw;
        }
        finally
        {
            _supplier = null;
            _isComputed = true;
        }

        _mutex.ReleaseMutex();
        return _result;
    }
}