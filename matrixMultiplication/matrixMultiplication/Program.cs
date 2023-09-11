using matrixMultiplication;


var a = new[,] {{1, 2, 4 }, {2, 3, 5}};
var b = new[,] { { 3, 8, 1, 1 }, {0, 3, 1, 1 }, {4, 5, 0, 3 } };
var a1 = new Matrix(a);
var b1 = new Matrix(b);

var c1 = Matrix.Multiply(a1, b1);

var d1 = Matrix.MultiplyInParallel(a1, b1);

var d2  = Matrix.MultiplyInParallel(a1, c1);