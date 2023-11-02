using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Client;

public class Client : IDisposable
{
    private readonly string _serverIp = "localhost";
    private readonly TcpClient _client;
    

    public Client(int port)
    {
        _client = new TcpClient(_serverIp, port);
    }

    public void Start()
    {
        var stream = _client.GetStream();
        while (true)
        {
            Console.WriteLine("Write request: ");
            var request = Console.ReadLine();
        }
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}