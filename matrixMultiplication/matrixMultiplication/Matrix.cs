namespace MatrixMultiplication;

using System.Text.RegularExpressions;


public class Matrix
{
    public int Rows => 
        Elements.GetLength(0);

    public int Columns => 
        Elements.GetLength(1);

    public int[,] Elements { get; }

    public Matrix(String path)
    {
        var matrix = new List<int[]>();
        using (var reader = new StreamReader(path))
        {
            while (reader.ReadLine() is { } line)
            {
                if (!IsCorrectMatrixLine(line))
                {
                    throw new FormatException("The file can only contain numbers");
                }
                var numbers = new Regex(@"-?\d+").Matches(line).
                    Select(match => int.Parse(match.Value)).ToArray();
                matrix.Add(numbers);
            }
        }
        if (matrix.Count == 0)
        {
            throw new ArgumentException("File is empty");
        }
        for (var i = 1; i < matrix.Count; i++)
        {
            if (matrix[i].Length != matrix[i - 1].Length)
            {
                throw new ArgumentException("the file must contain a matrix");
            }
        }
        var newElements = new int[matrix.Count, matrix[0].Length];
        for (int i = 0; i < newElements.GetLength(0); i++)
        {
            for (int j = 0; j < newElements.GetLength(1); j++)
            {
                newElements[i, j] = matrix[i][j];
            }
        }
        Elements = newElements;
    }

    public Matrix(int[,] array)
    {
        Elements = (int[,])array.Clone();
    }

    private bool IsCorrectMatrixLine(string line)
        => Regex.IsMatch(line, @"^(-?\d+ ?)+$");

    public static Matrix Multiply(Matrix leftMatrix, Matrix rightMatrix)
    {
        if (leftMatrix.Columns  != rightMatrix.Rows)
        {
            throw new ArgumentException("The number of columns of the left matrix " +
                "should be equal to the number of rows of the right matrix");
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

    public static Matrix MultiplyInParallel(Matrix leftMatrix, Matrix rightMatrix)
    {
        if (leftMatrix.Columns != rightMatrix.Rows)
        {
            throw new ArgumentException("The number of columns of the left matrix " +
                "should be equal to the number of rows of the right matrix");
        }
        var result = new int[leftMatrix.Rows, rightMatrix.Columns];
        var threads = new Thread[leftMatrix.Columns];
        for (int i = 0; i < leftMatrix.Rows; i++)
        {
            var row = i;
            threads[i] = new Thread(() =>
            {
                for (int j = 0; j < rightMatrix.Columns; j++)
                {
                    var sum = 0;
                    for (int k = 0; k < leftMatrix.Columns; k++)
                    {
                        sum += leftMatrix.Elements[row, k] * rightMatrix.Elements[k, j];
                    }
                    result[row, j] = sum;
                }
            });
            threads[i].Start();
        }
        for (int i = 0; i < result.GetLength(0); i++)
        {
            threads[i].Join();
        }
        return new Matrix(result);
    }

    public void WriteToFile(string path)
    {
        using var writer = new StreamWriter(path);
        for (var i = 0; i < Rows; i++)
        {
            for (var j = 0; j < Columns; j++)
            {
                writer.Write($"{Elements[i, j]} ");
            }
            writer.Write('\n');
        }
    }

    public static bool operator ==(Matrix left, Matrix right)
    {
        if (left.Columns != right.Columns || left.Rows != right.Rows)
        {
            return false;
        }

        for (var i = 0; i < left.Rows; ++i)
        {
            for (var j = 0; j < left.Columns; ++j)
            {
                if (left.Elements[i, j] != right.Elements[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool operator !=(Matrix left, Matrix right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj)
    {
        if ((obj == null) || GetType() != obj.GetType())
        {
            return false;
        }

        return (Matrix)obj == this;
    }

    public override int GetHashCode() =>
        base.GetHashCode();
}
