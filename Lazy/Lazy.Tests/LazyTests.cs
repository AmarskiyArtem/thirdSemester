namespace Lazy.Tests;

public class Tests
{
    private static readonly Random Rand = new();
    
    [TestCaseSource(nameof(NullFuncs))]
    public void NullFuncShouldException(Func<object> func)
    {
        Assert.Throws<ArgumentNullException>(() => new Lazy<object?>(func));
    }
    
    [TestCaseSource(nameof(LazyWithException))]
    public void FuncWithException(ILazy<object> lazy)
    {
        Assert.Throws<Exception>(() => lazy.Get());
    }

    
    [Test]
    public void MultithreadingTestShouldNoRaces()
    {
        int Func() => Rand.Next();
        var lazy = new LazyMultithreading<int>(Func);
        var list = new List<int>();
        var threads = new Thread[8];
        var threadHandler = new ManualResetEvent(false);
        for (var i = 0; i < threads.Length; ++i)
        {
            threads[i] = new Thread(() =>
            {
                threadHandler.WaitOne();
                list.Add(lazy.Get());
            });
        }
        
        foreach (var thread in threads)
        {
            thread.Start();
        }
        
        threadHandler.Set();
        
        foreach (var thread in threads)
        {
            thread.Join();
        }
        
        Assert.That(list.Distinct().Count(), Is.EqualTo(1));
    }

    [Test]
    public void LazyOneThreadShouldComputedOnce()
    {
        var lazy = new Lazy<int>(() => Rand.Next());
        Assert.That(lazy.Get() == lazy.Get(), Is.True);
    }

    private static IEnumerable<ILazy<object>> LazyWithException()
    {
        yield return new Lazy<object>(() => throw new Exception());
        yield return new LazyMultithreading<object>(() => throw new Exception());
    }

    private static IEnumerable<Func<object>> NullFuncs()
    {
        yield return null;
        yield return null;
    }
}