namespace Lazy;

public class Lazy<T>: ILazy<T>
{
    private Func<T> supplier;

    private bool isComputed;

    private T result;
    
    public Lazy(Func<T> func)
    {
        supplier = func;
    }
    
    public T? Get()
    {
        if (!isComputed)
        {
            result = supplier();
            isComputed = true;
        }

        return result;
    }
}