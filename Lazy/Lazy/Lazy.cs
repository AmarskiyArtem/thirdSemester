namespace Lazy;

public class Lazy<T>: ILazy<T>
{
    private Func<T>? supplier;

    private bool isComputed;

    private T? result;

    private Exception? supplierException;
    
    public Lazy(Func<T> func)
    {
        ArgumentNullException.ThrowIfNull(func);
        supplier = func;
    }
    
    public T? Get()
    {
        if (supplierException is null)
        {
            throw supplierException;
        }
        
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
                isComputed = true;
                supplier = null;
            }
        }

        return result;
    }
}