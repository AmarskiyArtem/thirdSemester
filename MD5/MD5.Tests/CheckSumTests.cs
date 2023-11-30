namespace MD5.Tests;

public class Tests
{
    public Tests()
    {
        Directory.SetCurrentDirectory("../../../..");
    }

    [Test]
    public void ParallelAndNotParallelShouldSameResult()
    {
        Assert.That(MD5.CheckSum.ComputeCheckSum("./"), Is.EqualTo(CheckSum.ComputeCheckSumParallel("./")));
    }
    
    [TestCaseSource(nameof(funcs))]
    public void SeveralLaunchesShouldSameResult(Func<string, byte[]> checkSum)
    {
        var firstResult = checkSum.Invoke("./MD5");
        for (var i = 0; i < 5; i++)
        {
            Assert.That(checkSum.Invoke("./MD5"), Is.EqualTo(firstResult));
        }
    }

    [TestCaseSource(nameof(funcs))]
    public void FileNotExistShouldException(Func<string, byte[]> checkSum)
        => Assert.Throws<ArgumentException>(() => checkSum.Invoke("NotExist.py"));
    

    [TestCaseSource(nameof(funcs))]
    public void DirectoryNotExistShouldException(Func<string, byte[]> checkSum)
        => Assert.Throws<ArgumentException>(() => checkSum.Invoke("./NotExist/"));
    
    private static Func<string, byte[]>[] funcs =
    {
        CheckSum.ComputeCheckSumParallel,
        CheckSum.ComputeCheckSum,
    };
    
}