namespace ThreadPool;

/// <summary>
/// Interface of an asynchronous task with a result of type TResult.
/// </summary>
public interface IMyTask<out TResult>
{
    /// <summary>
    /// Indicates whether it has been completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Contains the result of calculations.
    /// </summary>
    public TResult? Result { get; }

    /// <summary>
    /// Creates a task which will be executed after current.
    /// </summary>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult?, TNewResult> func);
}