namespace Lazy.Tests;

public class Tests
{
    private static readonly Random Rand = new();
    
    [Test]
    public void NullFuncShouldException()
    {
        Assert.Throws<ArgumentNullException>(() => new Lazy<object?>(null));
        Assert.Throws<ArgumentNullException>(() => new LazyMultithreading<object?>(null));
    }
    
    [TestCaseSource(nameof(LazyWithException))]
    public void FuncWithException(ILazy<object> lazy)
    {
        Assert.Throws<Exception>(() => lazy.Get());
    }

    
    [Test]
    public void MultithreadingTestShouldNoRaces()
    {
        var value = Rand.Next();
        var lazy = new LazyMultithreading<int>(() => value);
        var threads = new Thread[8];
        var threadHandler = new ManualResetEvent(false);
        for (var i = 0; i < threads.Length; ++i)
        {
            threads[i] = new Thread(() =>
            {
                threadHandler.WaitOne();
                Assert.That(lazy.Get(), Is.EqualTo(value));
            });
        }
        
        foreach (var thread in threads)
        {
            thread.Start();
        }
        
        threadHandler.Set();
    }

    [TestCaseSource(nameof(LazyObjects))]
    public void LazyShouldComputedOnce(ILazy<int> lazy)
    {
        Assert.That(lazy.Get(), Is.EqualTo(lazy.Get()));
    }

    private static IEnumerable<ILazy<object>> LazyWithException()
    {
        yield return new Lazy<object>(() => throw new Exception());
        yield return new LazyMultithreading<object>(() => throw new Exception());
    }
    
    private static IEnumerable<ILazy<int>> LazyObjects()
    {
        yield return new Lazy<int>(() => Rand.Next());
        yield return new LazyMultithreading<int>(() => Rand.Next());
    }
}
