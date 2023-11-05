namespace SimpleFTP;

using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    private CancellationTokenSource _cts = new();
    private readonly TcpListener _server;
    
    public Server(int port)
    {
        _server = new(IPAddress.Any, port);
    }

    public void Stop() =>
        _cts.Cancel();
    
    
    public async Task StartAsync()
    {
        try
        {
            _server.Start();
            var tasks = new List<Task>();
            while (!_cts.IsCancellationRequested)
            {
                var client = await _server.AcceptTcpClientAsync();
                tasks.Add(Task.Run(async () => await HandleClientAsync(client)));
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            _server.Stop();
            Clear();
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            while (client.Connected && _cts.IsCancellationRequested)
            {
                var buffer = new byte[client.ReceiveBufferSize];
                var count = await stream.ReadAsync(buffer);
                var request = Encoding.UTF8.GetString(buffer, 0, count).Split(" ");
                if (request.Length != 2)
                {
                    await SendDataAsync("-1"u8.ToArray(), stream);
                }

                switch (request[0])
                {
                    case "1":
                    {
                        await SendDataAsync(await ListAsync(request[1]), stream);
                        break;
                    }
                    case "2":
                    {
                        var bytes = await GetAsync(request[1]);
                        await SendDataAsync(BitConverter.GetBytes(bytes.Length), stream);
                        await SendDataAsync(bytes, stream);
                        break;
                    }
                    default:
                    {
                        await SendDataAsync("Unknown command"u8.ToArray(), stream);
                        break;
                    }
                }


            }
        }
        finally
        {
            client.Dispose();
            Clear();
        }
    }
    
    private async Task SendDataAsync(byte[] data, Stream stream)
    {
        await stream.WriteAsync(data);
        await stream.FlushAsync();
    }
    
    private async Task<byte[]> GetAsync(string path)
    {
        if (!File.Exists(path))
        {
            return "-1"u8.ToArray();
        }
        
        return await File.ReadAllBytesAsync(path);
    }
    
    private async Task<byte[]> ListAsync(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
        {
            return "-1"u8.ToArray();
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
        return Encoding.UTF8.GetBytes(response.ToString());
    }

    private void Clear()
    {
        _cts = new CancellationTokenSource();
    }
}