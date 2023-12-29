namespace FTPClient;

using System.Net.Sockets;

/// <summary>
/// Represents a simple FTP client.
/// </summary>
public class Client
{
    private readonly int _port;
    
    /// <summary>
    /// Initializes a new instance of the Client class with a specified port.
    /// </summary>
    /// <param name="port">The port number to connect to the server.</param>
    public Client(int port)
    {
        _port = port;
    }
    
    /// <summary>
    /// Starts the client to interact with the server by taking user input.
    /// </summary>
    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine("Write request: ");
            var request = Console.ReadLine();
            if (request != null && request.StartsWith("List "))
            {
                Console.WriteLine(await ListAsync(request.Substring(5))); 
            }
            else if (request != null && request.StartsWith("Get "))
            {
                Console.WriteLine(await GetAsync(request.Substring(4)));
            }
            else if (request == "exit")
            {
                return;
            }
            else
            {
                Console.WriteLine("Unknown command");
            }
        }
    }
    
    /// <summary>
    /// Sends a request to list files in dir on server.
    /// </summary>
    /// <param name="path">Path to dir on server</param>
    public async Task<string?> ListAsync(string path)
        => await SendRequestAsync($"1 {path}");
    
    /// <summary>
    /// Sends a request to get file bytes from server.
    /// </summary>
    /// <param name="path">Path to file on server</param>
    public async Task<string?> GetAsync(string path)
        => await SendRequestAsync($"2 {path}");

    private async Task<string?> SendRequestAsync(string request)
    {
        using var client = new TcpClient();
        await client.ConnectAsync("localhost", _port);
        await using var stream = client.GetStream();
        await using var writer = new StreamWriter(stream);
        using var reader = new StreamReader(stream);
        await writer.WriteLineAsync(request);
        await writer.FlushAsync();
        return await reader.ReadLineAsync();
    }
}