using System.Diagnostics;
using MD5;

var path = args[0];
try
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var result = CheckSum.ComputeCheckSumParallel(path);
    stopWatch.Stop();
    Console.WriteLine($"Check sum for {path} using not parallel method");
    Console.WriteLine(BitConverter.ToString(result));
    Console.WriteLine($"Time spent {stopWatch.Elapsed}");
    stopWatch.Reset();
    stopWatch.Start();
    result = CheckSum.ComputeCheckSumParallel(path);
    stopWatch.Stop();
    Console.WriteLine($"Check sum for {path} using parallel method");
    Console.WriteLine(BitConverter.ToString(result));
    Console.WriteLine($"Time spent {stopWatch.Elapsed}");
}
catch (ArgumentException)
{
    Console.WriteLine($"Cannot find {path}");
}
catch (IOException ex)
{
    Console.WriteLine(ex.Message);
}