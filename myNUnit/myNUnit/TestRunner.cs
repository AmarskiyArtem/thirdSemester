namespace myNUnit;

using System.Reflection;

public class TestRunner
{
    
    public TestRunner(string pathToAssembly) : this(Assembly.LoadFrom(pathToAssembly))
    {
        
    }

    public TestRunner(Assembly assembly)
    {
        
    }

    private Dictionary<Type, ClassInstances> ParseAssembly(Assembly assembly)
    {
        var instances = new Dictionary<Type, ClassInstances>();
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                foreach (var attribute in method.GetCustomAttributes())
                {
                    //if (attribute)
                }
            }
        }
    }
    
    public void Run()
    {
        
    }
}