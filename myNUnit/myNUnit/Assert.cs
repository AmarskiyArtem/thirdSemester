namespace myNUnit;

public static class Assert
{
    public static void That(bool predicate)
    {
        if (!predicate)
        {
            throw new AssertionErrorException();
        }
    }
}