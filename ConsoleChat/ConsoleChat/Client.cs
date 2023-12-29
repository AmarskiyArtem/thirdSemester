namespace ConsoleChat;

using System.Net.Sockets;

/// <summary>
/// Represents a client for a console chat application that connects to a remote server via TCP.
/// </summary>
public class Client
{
    private readonly TcpClient _client;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Client"/> class and connects to the specified host and port.
    /// </summary>
    /// <param name="host">The hostname or IP address of the server to connect to.</param>
    /// <param name="port">The port number to use for the connection.</param>
    public Client(string host, int port)
    {
        _client = new TcpClient(host, port);
    }

    /// <summary>
    /// Starts the chat client and begins communication with the server.
    /// </summary>
    public async Task Start()
    {
        var stream = _client.GetStream();
        var task = Task.Run(() =>
        {
            #pragma warning disable cs4014
            ChatHandler.Read(stream, _cancellationTokenSource);
            ChatHandler.Write(stream, _cancellationTokenSource);
        });
        task.Wait();
    }
}