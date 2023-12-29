namespace ThreadPool.Tests;

public class MyThreadPoolTests
{
    private MyThreadPool _threadPool = new(Environment.ProcessorCount);

    [SetUp]
    public void Setup()
    {
        _threadPool = new(Environment.ProcessorCount);
    }

    [Test]
    public void ResultAfterSubmitShouldExpectedValue()
    {
        var task = _threadPool.Submit(() => (2 + 2) * 3);
        Assert.That(task.Result, Is.EqualTo(12));
    }

    [Test]
    public void ResultAfterShutdownShouldExpectedValue()
    {
        var task = _threadPool.Submit(() => (2 + 2) * 3);
        _threadPool.Shutdown();
        Assert.That(task.Result, Is.EqualTo(12));
    }

    [Test]
    public void ContinueWithShouldExpectedValue()
    {
        for (var i = 0; i < 10; ++i)
        {
            var iCopy = i;
            var task = _threadPool.Submit(() => iCopy).ContinueWith(x => x + 1);
            Assert.That(task.Result, Is.EqualTo(iCopy + 1));
        }
    }

    [Test]
    public void SubmitAfterShutDownShouldThrowsException()
    {
        _threadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => _threadPool.Submit(() => 1));
    }

    [Test]
    public void ContinueWithAfterShutDownShouldThrowsException()
    {
        var task = _threadPool.Submit(() => 1);
        _threadPool.Shutdown();
        Assert.Throws<InvalidOperationException>(() => task.ContinueWith(x => x + 1));
    }
}