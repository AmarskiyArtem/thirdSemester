namespace FTPServer;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents a simple FTP server.
/// </summary>
public class Server
{
    private readonly TcpListener _server;
    private readonly CancellationTokenSource _cts = new();
    
    /// <summary>
    /// Initializes a new instance of the Server class.
    /// </summary>
    /// <param name="port">The port number to listen on.</param>
    public Server(int port)
    {
        _server = new TcpListener(IPAddress.Any, port);
    }
    
    /// <summary>
    /// Starts the server to accept incoming connections and process requests.
    /// </summary>
    public async Task StartAsync()
    {
        _server.Start();
        var tasks = new List<Task>();
        while (!_cts.Token.IsCancellationRequested)
        {
            var client = await _server.AcceptTcpClientAsync(_cts.Token);
            tasks.Add(Task.Run(async () =>
            {
                await using var stream = client.GetStream();
                using var reader = new StreamReader(stream);
                await using var writer = new StreamWriter(stream);

                while (await reader.ReadLineAsync() is { } request)
                {
                    if (request.StartsWith("1 "))
                    {
                        await ListAsync(request[2..], writer);
                    }

                    if (request.StartsWith("2 "))
                    {
                        await GetAsync(request[2..], writer);
                    }
                }

                client.Close();
            }));
        }
        
        Task.WaitAll(tasks.ToArray());
    }
    
    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        _cts.Cancel();
        _server.Stop();
    }

    private static async Task ListAsync(string path, TextWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var entries = Directory.GetFileSystemEntries(path);
        Array.Sort(entries);

        await writer.WriteAsync($"{entries.Length}");
        foreach (var entry in entries)
        {
            var isDirectory = Directory.Exists(entry);
            await writer.WriteAsync($" {Path.GetFileName(entry)} {isDirectory}");
        }
        await writer.WriteLineAsync();
        await writer.FlushAsync();
    }

    private static async Task GetAsync(string path, TextWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteLineAsync("-1");
            await writer.FlushAsync();
            return;
        }

        var content = await File.ReadAllBytesAsync(path);
        var contentHex = BitConverter.ToString(content).Replace("-", "");
        await writer.WriteLineAsync($"{content.Length} {contentHex}");
        await writer.FlushAsync();
    }
}