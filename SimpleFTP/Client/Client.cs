using System.Net.Sockets;
using System.Text;

namespace Client;

public class Client : IDisposable
{
    private readonly string _serverIp = "localhost";
    private readonly TcpClient _client;
    

    public Client(int port)
    {
        _client = new TcpClient(_serverIp, port);
    }

    /*public void Start()
    {
        var stream = _client.GetStream();
        while (true)
        {
            Console.WriteLine("Write request: ");
            var request = Console.ReadLine();
        }
    }*/

    public async Task<byte[]> GetAsync(string path, Stream stream)
    {
        await stream.WriteAsync("2"u8.ToArray());
        await stream.WriteAsync(Encoding.UTF8.GetBytes(path));
        await stream.WriteAsync("\n"u8.ToArray());
        await stream.FlushAsync();
        var buffer = new byte[_client.ReceiveBufferSize];
        await stream.ReadAsync(buffer);
        return buffer;
    }

    public async Task<string> ListAsync(string path, Stream stream)
    {
        await stream.WriteAsync("1"u8.ToArray());
        await stream.WriteAsync(Encoding.UTF8.GetBytes(path));
        await stream.WriteAsync("\n"u8.ToArray());
        var buffer = new byte[_client.ReceiveBufferSize];
        var data = await stream.ReadAsync(buffer);
        return Encoding.UTF8.GetString(buffer, 0, data);
    } 
    
    public void Dispose()
    {
        _client.Dispose();
    }
}