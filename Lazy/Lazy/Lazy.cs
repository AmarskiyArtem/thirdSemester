namespace Lazy;

/// <summary>
/// Implementation of ILazy interface
/// </summary>
public class Lazy<T>: ILazy<T>
{
    private Func<T>? _supplier;

    private bool _isComputed;

    private T? _result;

    private Exception? _supplierException;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Lazy{T}"/> class with the specified value supplier function.
    /// </summary>
    /// <param name="func">The function that supplies the value when requested.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="func"/> is null.</exception>
    public Lazy(Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        _supplier = func;
    }
    
    /// <inheritdoc/>
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