namespace ConsoleChat;

public static class ChatHandler
{
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