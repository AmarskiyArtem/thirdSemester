namespace matrixMultiplication;

using System.IO;

public class Matrix
{

    public int Rows { get; private set; }
    public int Columns { get; private set; }

    public int[,] Elements { get; private set; }

    public Matrix(String path)
    {
        try
        {
            var lines = File.ReadAllLines(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public Matrix(int[,] array)
    {
        Rows = array.GetLength(0);
        Columns = array.GetLength(1);
        Elements = (int[,])array.Clone();
    }

    private int ScalarProduct(int[] row, int[] column)
    {
        var result = 0;
        for (int i = 0; i < row.Length; ++i)
        {
            result += row[i] * column[i];
        }
        return result;
    }

    public static Matrix Multiply(Matrix leftMatrix, Matrix rightMatrix)
    {
        if (leftMatrix.Columns  != rightMatrix.Rows)
        {
            throw new ArgumentException();
        }
        var result = new int[leftMatrix.Rows, rightMatrix.Columns];
        for (int i = 0; i < leftMatrix.Rows; i++)
        {
            for (int j = 0; j < rightMatrix.Columns; j++)
            {
                for (int k = 0; k < leftMatrix.Columns; k++)
                {
                    result[i, j] += leftMatrix.Elements[i, k] * rightMatrix.Elements[k, j];
                }
            }
        }
        return new Matrix(result);
    }

    public static Matrix MultiplyInParallel(Matrix leftMatrix, Matrix secondMatrix) { return null; }


}
