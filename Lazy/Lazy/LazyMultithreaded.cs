namespace Lazy;

public class LazyMultithreading<T> : ILazy<T>
{
    public LazyMultithreading
    
    private Object lockObject = new();
    
    public T? Get()
    {
        throw new NotImplementedException();
    }
}