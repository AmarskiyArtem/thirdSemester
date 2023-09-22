namespace MatrixMultiplication;

/// <summary>
/// A utility class for working with matrices.
/// </summary>
public static class MatrixUtils
{
    private static Random rand = new();
    
    /// <summary>
    /// Generates a random integer matrix with the specified number of rows and columns.
    /// </summary>
    /// <returns>A Matrix object representing the generated integer matrix.</returns>
    public static Matrix CreateIntMatrix(int numberOfRows, int numberOfColumns)
    {
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