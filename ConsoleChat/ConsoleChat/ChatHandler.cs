namespace ConsoleChat;

public static class ChatHandler
{
    public static Task Read(Stream stream, CancellationTokenSource cts, Action exit)
    {
        Task.Run(async () =>
        {
            using var streamReader = new StreamReader(stream);
            while (!cts.IsCancellationRequested)
            {
                var message = await streamReader.ReadLineAsync();
                if (message == "exit")
                {
                    break;
                }
                Console.WriteLine(message);
            }
            exit.Invoke();
        });
        return Task.CompletedTask;
    }

    public static Task Write(Stream stream, CancellationTokenSource cts, Action exit)
    {
        return Task.Run(async () =>
        {
            await using var streamWriter = new StreamWriter(stream);
            while (!cts.IsCancellationRequested)
            {
                var message = Console.ReadLine();
                if (message == "exit")
                {
                    break;
                }
                
                await streamWriter.WriteLineAsync(message);
                await streamWriter.FlushAsync();
            }
            exit.Invoke();
        });
    }
}