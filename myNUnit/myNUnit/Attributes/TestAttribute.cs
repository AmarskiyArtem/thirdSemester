namespace myNUnit;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class TestAttribute : Attribute
{
    public TestAttribute(Type? expected)
    {
        Ignore = null;
        Expected = expected;
    }

    public TestAttribute(string ignore, Type? expected)
    {
        Ignore = ignore;
        Expected = expected;
    }
    
    public Type? Expected { get; private set; }
    
    public string? Ignore { get; private set; }
}