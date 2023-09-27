namespace Lazy;

public class Lazy<T>: ILazy<T>
{
    private Func<T>? _supplier;

    private bool _isComputed;

    private T? _result;

    private Exception? _supplierException;
    
    public Lazy(Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        _supplier = func;
    }
    
    public T? Get()
    {
        if (_supplierException is not null)
        {
            throw _supplierException;
        }
        
        if (!_isComputed)
        {
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
                _isComputed = true;
                _supplier = null;
            }
        }

        return _result;
    }
}