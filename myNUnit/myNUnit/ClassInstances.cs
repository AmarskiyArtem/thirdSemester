namespace myNUnit;

using System.Reflection;

public record ClassInstances
{
    public MethodInfo[] BeforeClass;
    public MethodInfo[] AfterClass;
    public MethodInfo[] Tests;
    public MethodInfo[] BeforeTest;
    public MethodInfo[] AfterTest;
}