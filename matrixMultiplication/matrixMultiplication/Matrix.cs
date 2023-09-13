﻿using System.Text.RegularExpressions;

namespace matrixMultiplication;


public class Matrix
{

    public int Rows { get; }
    public int Columns { get; }

    public int[,] Elements { get; }

    public Matrix(String path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }
        var matrix = new List<int[]>();
        using (var reader = new StreamReader(path))
        {
            while (reader.ReadLine() is string line)
            {
                if (!IsCorrectMatrixLine(line))
                {
                    throw new FormatException();
                }
                var numbers = new Regex(@"-?\d+").Matches(line).
                    Select(match => int.Parse(match.Value)).ToArray();
                matrix.Add(numbers);
            }
        }
        if (matrix.Count == 0)
        {
            throw new ArgumentException();
        }
        for (var i = 1; i < matrix.Count; i++)
        {
            if (matrix[i].Length != matrix[i - 1].Length)
            {
                throw new ArgumentException();
            }
        }
        Rows = matrix.Count;
        Columns = matrix[0].Length;
        var newElements = new int[Rows, Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                newElements[i, j] = matrix[i][j];
            }
        }
        Elements = newElements;
    }

    public Matrix(int[,] array)
    {
        Rows = array.GetLength(0);
        Columns = array.GetLength(1);
        Elements = (int[,])array.Clone();
    }

    private bool IsCorrectMatrixLine(string line) 
        => Regex.IsMatch(line, @"^(-?\d+ ?)+");


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

    public static Matrix MultiplyInParallel(Matrix leftMatrix, Matrix rightMatrix)
    {
        if (leftMatrix.Columns != rightMatrix.Rows)
        {
            throw new ArgumentException();
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

}
