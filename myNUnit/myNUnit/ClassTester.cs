namespace myNUnit;

using System.Reflection;

public class ClassTester
{
    private readonly Type _type;

    private readonly ClassInstances _classInstances;
    
    public ClassTester(Type type)
    {
        _type = type;
        _classInstances = ParseClass();
    }

    private bool isCorrectMethod(MethodInfo method)
        => method.GetParameters().Length == 0 && method.ReturnType == typeof(void); 
    
    private ClassInstances ParseClass()
    {
        var classInstances = new ClassInstances();
        foreach (var method in  _type.GetMethods())
        {
            foreach (var attribute in method.GetCustomAttributes())
            {
                if (attribute is BeforeAttribute && isCorrectMethod(method))
                {
                    classInstances.BeforeTest.Append(method);
                }
                
                if (attribute is AfterAttribute && isCorrectMethod(method))
                {
                    classInstances.AfterTest.Append(method);
                }
                
                if (attribute is TestAttribute && isCorrectMethod(method))
                {
                    classInstances.Tests.Append(method);
                }
                
                if (attribute is BeforeClassAttribute && isCorrectMethod(method))
                {
                    classInstances.BeforeClass.Append(method);
                }
                
                if (attribute is AfterClassAttribute && isCorrectMethod(method))
                {
                    classInstances.AfterClass.Append(method);
                }
            }
        }

        return classInstances;
    }

    public void RunTests()
    {
        foreach (var method in _classInstances.BeforeClass)
        {
            method.Invoke();
        }
    }
}