namespace Lazy.Tests;

public class Tests
{
    private static readonly Random Rand = new();
    
    [Test]
    public void NullFuncShouldException()
    {
        Assert.Throws<ArgumentNullException>(() => new Lazy<object?>(null!));
        Assert.Throws<ArgumentNullException>(() => new LazyMultithreading<object>(null!));
    }

    [Test]
    public void FuncWithException()
    {
        Assert.Throws<Exception>(() =>
        {
            var lazy = new Lazy<object>(() => throw new Exception());
            lazy.Get();
        });
        Assert.Throws<Exception>(() =>
        {
            var lazy = new LazyMultithreading<object>(() => throw new Exception());
            lazy.Get();
        });
    }

    [Test]
    public void MultithreadingTestShouldNoRaces()
    {
        int Func() => Rand.Next();
        var lazy = new LazyMultithreading<int>(Func);
        var list = new List<int>();
        var threads = new Thread[Environment.ProcessorCount];
        for (var i = 0; i < threads.Length; ++i)
        {
            threads[i] = new Thread(() => list.Add(lazy.Get()));
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
        
        Assert.That(list.Distinct().Count(), Is.EqualTo(1));
    }
}