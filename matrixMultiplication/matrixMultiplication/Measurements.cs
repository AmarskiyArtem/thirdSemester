namespace MatrixMultiplication;

using System.Diagnostics;

public class Measurements
{
    private int[,] dimensions = new [,] { { 100, 200 }, { 200, 200 }, { 400, 500 }, { 600, 600 } };
    
    public void MeasureMultiplication(string path)
    {
        var resultNotParallel = new long[4][];
        var resultParallel = new long[4][];
        for (var i = 0; i < 4; ++i)
        {
            resultNotParallel[i] = new long[5];
            resultParallel[i] = new long[5];
        }
        var stopWatch = new Stopwatch();
        for (var i = 0; i < 4; ++i)
        {
            var leftMatrix = MatrixUtils.CreateIntMatrix(dimensions[i, 0], dimensions[i, 1]);
            var rightMatrix = MatrixUtils.CreateIntMatrix(dimensions[i, 1], dimensions[i, 0]);
            
            for (var j = 0; j < 5; j++)
            {
                stopWatch.Start();
                Matrix.MultiplyInParallel(leftMatrix, rightMatrix);
                stopWatch.Stop();
                resultParallel[i][j] = stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();
            }

            for (var j = 0; j < 5; ++j)
            {
                stopWatch.Start();
                Matrix.Multiply(leftMatrix, rightMatrix);
                stopWatch.Stop();
                resultNotParallel[i][j] = stopWatch.ElapsedMilliseconds;
                stopWatch.Reset();
            }
        }

        var statisticsNotParallel = new double[4, 2];
        var statisticsParallel = new double[4, 2];
        for (var i = 0; i < resultNotParallel.GetLength(0); ++i)
        {
            statisticsNotParallel[i, 0] = GetMathExpectation(resultNotParallel[i]);
            statisticsNotParallel[i, 1] = GetStandardDeviation(resultNotParallel[i]);
            statisticsParallel[i, 0] = GetMathExpectation(resultParallel[i]);
            statisticsParallel[i, 1] = GetStandardDeviation(resultParallel[i]);
        }
        WriteResultToFile(statisticsNotParallel, statisticsParallel, path);
    }

    private void WriteResultToFile(double[,] statisticsNotParallel, double[,] statisticsParallel, string path)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine("Size 1|Size 2|Par\\NonPar MathExp|Par\\NonPar StandDev");
        for (var i = 0; i < statisticsParallel.GetLength(0); ++i)
        {
            var str = $"{dimensions[i, 0]}x{dimensions[i, 1]}";
            var str2 = $"{dimensions[i, 1]}x{dimensions[i, 0]}";
            writer.WriteLine($"{str:<8} {str2:<8} {statisticsParallel[i, 0].ToString():<4}/" +
                             $"{statisticsNotParallel[i, 0].ToString():<4}" +
                         $" {statisticsParallel[i, 1].ToString():<4}/" +
                             $"{statisticsNotParallel[i, 1].ToString():<4}");
        }
    }
    
    private double GetMathExpectation(long[] results) =>
        results.Sum(t => t * 1d / results.GetLength(0));
    
    
    private double GetStandardDeviation(long[] results)
    {
        var mathExpectation = GetMathExpectation(results);
        var standardDeviation = results.Sum(t => (t - mathExpectation) * (t - mathExpectation));
        return Math.Sqrt(standardDeviation / (results.Length - 1));
    }
}