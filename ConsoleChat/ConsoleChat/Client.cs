using System.Net.Sockets;

namespace ConsoleChat;

public class Client
{
    private readonly TcpClient _client;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    public Client(string host, int port)
    {
        _client = new TcpClient(host, port);
    }

    public async Task Start()
    {
        var stream = _client.GetStream();
        await ChatHandler.Read(stream, _cancellationTokenSource, Exit);
        await ChatHandler.Write(stream, _cancellationTokenSource, Exit);
    }

    private void Exit()
    {
        _client.Close();
    }
}