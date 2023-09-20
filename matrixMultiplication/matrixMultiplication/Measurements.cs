namespace MatrixMultiplication;

using System.Diagnostics;

public class Measurements
{
    public void MeasureMultiplication()
    {
        var resultNotParallel = new long[4][];
        for (var i = 0; i < 5; ++i)
        {
            resultNotParallel[i] = new long[5];
        }
        var resultParallel = new long[4][];
        for (var i = 0; i < 5; ++i)
        {
            resultParallel[i] = new long[5];
        }
        var dimensions = new [,] { { 100, 200 }, { 200, 200 }, { 400, 500 }, { 600, 600 } };
        for (var i = 0; i < 4; ++i)
        {
            var stopWatch = new Stopwatch();
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
        for (var i = 0; i < resultNotParallel.GetLength(0); ++i)
        {
            statisticsNotParallel[i, 0] = GetMathExpectation(resultNotParallel[i]);
            statisticsNotParallel[i, 1] = GetStandardDeviation(resultNotParallel[i]);
        }
        var statisticsParallel = new double[4, 2];
        for (var i = 0; i < resultNotParallel.GetLength(0); ++i)
        {
            statisticsParallel[i, 0] = GetMathExpectation(resultParallel[i]);
            statisticsParallel[i, 1] = GetStandardDeviation(resultParallel[i]);
        }
    }

    public void MakeResultInStringFormat(double[,] statisticsNotParallel, double[,] statisticsParallel)
    {
        
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