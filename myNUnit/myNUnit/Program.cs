using System.Reflection;
using myNUnit;

/*if (args.Length != 1)
{
    Console.WriteLine("Enter the path to test project");
}
else
{
    
}*/
var a = Assembly.LoadFrom(@"C:\myFiles\programming\university\thirdSemester\matrixMultiplication\matrixMultiplication\bin\Debug\net8.0\matrixMultiplication.dll");
Type[] types = a.GetTypes();

// Выводим информацию о каждом найденном классе
foreach (Type type in types)
{
    Console.WriteLine("Найден класс: " + type.FullName);
}