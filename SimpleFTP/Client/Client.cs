namespace Client;

using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

public class Client : IDisposable
{
    private readonly string _serverIp = "localhost";
    private readonly TcpClient _client;
    
    public Client(int port)
    {
        _client = new TcpClient(_serverIp, port);
    }

    public async Task StartAsync()
    {
        var stream = _client.GetStream();
        while (true)
        {
            Console.WriteLine("Write request: ");
            var request = Console.ReadLine();
            if (request != null && request.StartsWith("List "))
            {
                Console.WriteLine(await ListAsync(request.Substring(5), stream)); 
            }
            else if (request != null && request.StartsWith("Get "))
            {
                Console.WriteLine(await GetAsync(request.Substring(4), stream));
            }
            else
            {
                Console.WriteLine("Unknown command");
            }
        }
    }

    public async Task<byte[]> GetAsync(string path, Stream stream)
    {
        await stream.WriteAsync("2"u8.ToArray());
        await stream.WriteAsync(Encoding.UTF8.GetBytes(path));
        await stream.WriteAsync("\n"u8.ToArray());
        await stream.FlushAsync();
        return await GetAnswerAsync(stream);
    }
    
    private async Task<byte[]> GetAnswerAsync(Stream stream)
    {
        var bytes = new List<byte>();
        byte bt;
        while ((bt = (byte)stream.ReadByte()) != ' ')
        {
            bytes.Add(bt);
        }

        var size = BitConverter.ToInt32(bytes.ToArray());
        var data = new byte[size];
        var count = await stream.ReadAsync(data);
        if (count != size)
        {
            throw new NetworkInformationException();
        }

        return data;
    }
    
    public async Task<string> ListAsync(string path, Stream stream)
    {
        await stream.WriteAsync("1"u8.ToArray());
        await stream.WriteAsync(Encoding.UTF8.GetBytes(path));
        await stream.WriteAsync("\n"u8.ToArray());
        return await ListAnswerAsync(stream);
    }

    private async Task<string> ListAnswerAsync(Stream stream)
    {
        var bytes = new List<byte>();
        byte bt;
        while ((bt = (byte)stream.ReadByte()) != ' ')
        {
            bytes.Add(bt);
        }

        var expectedSize = BitConverter.ToInt32(bytes.ToArray());
        bytes.Clear();
        var size = 0;
        int tmp;
        while ((tmp = stream.ReadByte()) != -1)
        {
            bytes.Add((byte)tmp);
            if ((byte)tmp == '\n')
            {
                ++size;
            }
        }

        if (expectedSize != size)
        {
            throw new NetworkInformationException();
        }
        
        return Encoding.UTF8.GetString(bytes.ToArray());
    }
    
    public void Dispose()
    {
        _client.Dispose();
    }
}