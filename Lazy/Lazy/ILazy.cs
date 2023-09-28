namespace Lazy;

/// <summary>
/// Represents a generic interface for lazily retrieving a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of value to be lazily retrieved.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets the lazily retrieved value of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>The lazily retrieved value of type <typeparamref name="T"/>, or null if the value is not available.</returns>
    T? Get();
}