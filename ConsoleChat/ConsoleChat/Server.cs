using System.Net;
using System.Net.Sockets;

namespace ConsoleChat;

public class Server
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private readonly TcpListener _server;

    public Server(int port)
    {
        _server = new TcpListener(IPAddress.Any, port);
    }

    public async Task Start()
    {
        _server.Start();
        Console.WriteLine("Server started");
        var client = await _server.AcceptTcpClientAsync();
        var stream = client.GetStream();
        Console.WriteLine("Client connected");
        ChatHandler.Read(stream, _cancellationTokenSource, Exit);
        await ChatHandler.Write(stream, _cancellationTokenSource, Exit);
    }

    private void Exit()
    {
        _server.Stop();
    }
    
}