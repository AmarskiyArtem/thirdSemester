namespace MatrixMultiplication.Tests;

public class MatrixTests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void MultiplicationShouldRightResult()
    {
        var left = new Matrix(new[,] { { 1, 2, 4, 1 }, { 3, 0, 0, 14 }, { 3, 1, 2, 0 } });
        var right = new Matrix(new[,] { { 1, 0 }, { 12, 78 }, { 0, 0 }, { 23, 0 } });
        Assert.That(Matrix.Multiply(left, right),
            Is.EqualTo(new Matrix(new[,] { { 48, 156 }, { 325, 0 }, { 15, 78 } })));
    }

    [Test]
    public void MultiplicationInParallelShouldRightResult()
    {
        var left = new Matrix(new[,] { { 1, 4, 3 }, { 5, 0, 0 }, { 12, 1, 0 } });
        var right = new Matrix(new[,] { { 0 }, { 1 }, { 5 } });
        Assert.That(Matrix.Multiply(left, right),
            Is.EqualTo(new Matrix(new[,] { {19}, {0}, {1} })));
    }

    [Test]
    public void MultiplicationBothTypesShouldEqualResults()
    {
        for (var i = 0; i < 10; ++i)
        {
            var left = MatrixUtils.CreateIntMatrix(10, 15);
            var right = MatrixUtils.CreateIntMatrix(15, 20);
            Assert.That(Matrix.Multiply(left, right) == Matrix.MultiplyInParallel(left, right), Is.True);
        }
    }

    [Test]
    public void ReadFromFileShouldCorrectMatrix()
    {
        var correctMatrix = new Matrix(new[,] { { 23, -12, 35, 541 }, { 0, 11, 1111, -123 }, { 0, 23123, -1234, 44 } });
        Assert.That(correctMatrix == new Matrix(@"../../../CorrectTestMatrix.txt"), Is.True);
    }

    [Test]
    public void ReadFromNotExistingFileShouldException()
    {
        Assert.Throws<FileNotFoundException>(() => new Matrix(@"NotExisting.txt"));
    }

    [Test]
    public void IncorrectMatrixShouldException()
    {
        Assert.Throws<ArgumentException>(() => new Matrix(@"../../../IncorrectMatrix.txt"));
    }
}



