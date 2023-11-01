namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    private readonly int _port;
    private readonly CancellationTokenSource _cts = new();
    private readonly TcpListener _listener;
    
    public Server(int port)
    {
        _port = port;
        _listener = new(IPAddress.Any, _port);
    }

    private async Task GetAsync(string path, StreamWriter streamWriter)
    {
        if (!File.Exists(path))
        {
            streamWriter.WriteAsync("-1");
            streamWriter.FlushAsync();
            return;
        }

        var data = await File.ReadAllBytesAsync(path, _cts.Token);
        var dataString = BitConverter.ToString(data);
        streamWriter.WriteAsync($"{dataString.Length} {dataString}");
        streamWriter.FlushAsync();
    }

    private async Task ListAsync(string path, StreamWriter streamWriter)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
        {
            streamWriter.WriteAsync("-1");
            streamWriter.FlushAsync();
            return;
        }
        
        var response = new StringBuilder();
        response.Append($"{directoryInfo.GetFiles().Length + directoryInfo.GetDirectories().Length} ");
        foreach (var file in directoryInfo.GetFiles())
        {
            response.Append($"{file} false");
        }

        foreach (var directory in directoryInfo.GetDirectories())
        {
            response.Append($"{directory} true");
        }

        response.Append("\n");
        streamWriter.WriteAsync(response.ToString());
        streamWriter.FlushAsync();
    }
}