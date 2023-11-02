namespace ConsoleChat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents a server for a console chat application that listens for incoming client connections via TCP.
/// </summary>
public class Server
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly TcpListener _server;

    /// <summary>
    /// Initializes a new instance of the <see cref="Server"/> class and starts listening for incoming client connections on the specified port.
    /// </summary>
    /// <param name="port">The port number on which the server should listen for incoming connections.</param>
    public Server(int port)
    {
        _server = new TcpListener(IPAddress.Any, port);
    }

    /// <summary>
    /// Starts the chat server and listens for incoming client connections.
    /// </summary>
    public async Task Start()
    {
        _server.Start();
        Console.WriteLine("Server started");
        var client = await _server.AcceptTcpClientAsync();
        var stream = client.GetStream();
        Console.WriteLine("Client connected");
        var task = Task.Run(() =>
        {
            #pragma warning disable cs4014
            ChatHandler.Read(stream, _cancellationTokenSource);
            ChatHandler.Write(stream, _cancellationTokenSource);
        });
        task.Wait();
    }
}