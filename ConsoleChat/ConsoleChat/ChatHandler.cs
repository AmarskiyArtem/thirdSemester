namespace ConsoleChat;

/// <summary>
/// Provides utility methods for reading and writing messages in a chat application using a stream.
/// </summary>
public static class ChatHandler
{
    /// <summary>
    /// Reads messages from the provided stream and displays them in the console.
    /// </summary>
    /// <param name="stream">The input stream from which messages are read.</param>
    /// <param name="cts">A CancellationTokenSource for possible cancellation of the operation.</param>
    /// <returns>A Task representing the asynchronous read operation.</returns>
    public static Task Read(Stream stream, CancellationTokenSource cts)
    {
        Task.Run(async () =>
        {
            using var streamReader = new StreamReader(stream);
            while (!cts.IsCancellationRequested)
            {
                var message = await streamReader.ReadLineAsync();
                if (message == "exit")
                {
                    await cts.CancelAsync();
                    Environment.Exit(0);
                }
                Console.WriteLine(message);
            }
        });
        return Task.CompletedTask;
    }

    /// <summary>
    /// Writes messages from the console to the provided output stream.
    /// </summary>
    /// <param name="stream">The output stream to which messages are written.</param>
    /// <param name="cts">A CancellationTokenSource for possible cancellation of the operation.</param>
    /// <returns>A Task representing the asynchronous write operation.</returns>
    public static Task Write(Stream stream, CancellationTokenSource cts)
    {
        return Task.Run(async () =>
        {
            await using var streamWriter = new StreamWriter(stream);
            while (!cts.IsCancellationRequested)
            {
                var message = Console.ReadLine();
                if (message == "exit")
                {
                    await cts.CancelAsync();
                }
                
                await streamWriter.WriteLineAsync(message);
                await streamWriter.FlushAsync();
            }
        });
    }
}