namespace FTPClient;

using System.Net.Sockets;

public class Client
{
    private readonly int _port;
    
    public Client(int port)
    {
        _port = port;
    }
    
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
    
    public async Task<string?> ListAsync(string path)
        => await SendRequestAsync($"1 {path}");
    
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