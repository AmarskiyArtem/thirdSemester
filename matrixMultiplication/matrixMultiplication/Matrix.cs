namespace MatrixMultiplication;

using System.Text.RegularExpressions;

/// <summary>
/// Represents a matrix and provides operations for matrix multiplication.
/// </summary>
public partial class Matrix
{
    /// <summary>
    /// Gets the number of rows in the matrix.
    /// </summary>
    public int Rows => 
        Elements.GetLength(0);

    /// <summary>
    /// Gets the number of columns in the matrix.
    /// </summary>
    public int Columns => 
        Elements.GetLength(1);

    /// <summary>
    /// Gets the two-dimensional array of elements representing the matrix.
    /// </summary>
    public int[,] Elements { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class from a file.
    /// </summary>
    /// <param name="path">The path to the file containing the matrix data.</param>
    /// <exception cref="FormatException">Thrown when the file contains non-numeric data.</exception>
    /// <exception cref="ArgumentException">Thrown when the file is empty or does not represent a valid matrix.</exception>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class from a two-dimensional array.
    /// </summary>
    /// <param name="array">The two-dimensional array representing the matrix.</param>
    public Matrix(int[,] array)
    {
        Elements = (int[,])array.Clone();
    }

    private static bool IsCorrectMatrixLine(string line)
        => MyRegex().IsMatch(line);

    /// <summary>
    /// Performs matrix multiplication between two matrices.
    /// </summary>
    /// <param name="leftMatrix">The left matrix.</param>
    /// <param name="rightMatrix">The right matrix.</param>
    /// <returns>The result of matrix multiplication.</returns>
    /// <exception cref="ArgumentException">Thrown when the matrix dimensions are incompatible for multiplication.</exception>
    public static Matrix Multiply(Matrix leftMatrix, Matrix rightMatrix)
    {
        if (leftMatrix.Columns  != rightMatrix.Rows)
        {
            throw new ArgumentException("The number of columns of the left matrix " +
                "should be equal to the number of rows of the right matrix");
        }
        var result = new int[leftMatrix.Rows, rightMatrix.Columns];
        for (var i = 0; i < leftMatrix.Rows; i++)
        {
            for (var j = 0; j < rightMatrix.Columns; j++)
            {
                for (var k = 0; k < leftMatrix.Columns; k++)
                {
                    result[i, j] += leftMatrix.Elements[i, k] * rightMatrix.Elements[k, j];
                }
            }
        }
        return new Matrix(result);
    }

    /// <summary>
    /// Performs matrix multiplication between two matrices in parallel.
    /// </summary>
    /// <param name="leftMatrix">The left matrix.</param>
    /// <param name="rightMatrix">The right matrix.</param>
    /// <returns>The result of matrix multiplication.</returns>
    /// <exception cref="ArgumentException">Thrown when the matrix dimensions are incompatible for multiplication.</exception>
    public static Matrix MultiplyInParallel(Matrix leftMatrix, Matrix rightMatrix)
    {
        if (leftMatrix.Columns != rightMatrix.Rows)
        {
            throw new ArgumentException("The number of columns of the left matrix " +
                                        "should be equal to the number of rows of the right matrix");
        }

        var result = new int[leftMatrix.Rows, rightMatrix.Columns];

        Thread[] threads;
        if (Environment.ProcessorCount > leftMatrix.Rows)
        {
            threads = new Thread[leftMatrix.Rows];
            for (var i = 0; i < threads.Length; ++i)
            {
                var row = i;
                threads[i] = new Thread(() =>
                {
                    for (var j = 0; j < rightMatrix.Columns; j++)
                    {
                        var sum = 0;
                        for (var k = 0; k < leftMatrix.Columns; k++)
                        {
                            sum += leftMatrix.Elements[row, k] * rightMatrix.Elements[k, j];
                        }

                        result[row, j] = sum;
                    }
                });
                threads[i].Start();
            }
        }
        else
        {
            threads = new Thread[Environment.ProcessorCount];
            var unitSize = leftMatrix.Rows / Environment.ProcessorCount;
            for (var i = 0; i < Environment.ProcessorCount; ++i)
            {
                var unitStart = i * unitSize;
                var unitEnd = (i == threads.Length - 1) ? leftMatrix.Rows : unitStart + unitSize;
                threads[i] = new Thread(() => MultiplyByUnits(unitStart, unitEnd, leftMatrix, rightMatrix, result));
                threads[i].Start();
            }
        }

        foreach (var t in threads)
        {
            t.Join();
        }
        return new Matrix(result);
    }
    
    private static void MultiplyByUnits(int start, int end, Matrix leftMatrix, Matrix rightMatrix, int[,] result)
    {
        for (var i = start; i < end; ++i)
        {
            for (var j = 0; j < rightMatrix.Columns; ++j)
            {
                for (var k = 0; k < leftMatrix.Columns; ++k)
                {
                    result[i, j] += leftMatrix.Elements[i, k] * rightMatrix.Elements[k, j];
                }

            }
        }
    }
    
    /// <summary>
    /// Writes the matrix data to a file.
    /// </summary>
    /// <param name="path">The path to the file where the matrix data will be written.</param>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public static bool operator !=(Matrix left, Matrix right)
    {
        return !(left == right);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if ((obj == null) || GetType() != obj.GetType())
        {
            return false;
        }

        return (Matrix)obj == this;
    }

    /// <inheritdoc/>
    public override int GetHashCode() =>
        base.GetHashCode();
    
    [GeneratedRegex(@"^-?\d+ ?( -?\d+)*$")]
    private static partial Regex MyRegex();
}
