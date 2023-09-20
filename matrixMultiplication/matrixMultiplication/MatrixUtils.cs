namespace MatrixMultiplication;

public static class MatrixUtils
{
    public static Matrix CreateIntMatrix(int numberOfRows, int numberOfColumns)
    {
        var rand = new Random();
        var matrix = new int[numberOfRows, numberOfColumns];
        for (var i = 0; i < matrix.GetLength(0); ++i)
        {
            for (var j = 0; j < matrix.GetLength(1); ++j)
            {
                matrix[i, j] = rand.Next(-100, 100);
            }
        }

        return new Matrix(matrix);
    }
}