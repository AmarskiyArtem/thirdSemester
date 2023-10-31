using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        int port = 7000;
        TcpListener server = new TcpListener(IPAddress.Any, 7000);

        server.Start();
        Console.WriteLine("Сервер запущен. Ожидание подключения клиента...");

        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Клиент подключен.");

        NetworkStream stream = client.GetStream();
        byte[] data = new byte[client.ReceiveBufferSize];
        int bytes;

        while (true)
        {
            bytes = stream.Read(data, 0, data.Length);
            string message = Encoding.Unicode.GetString(data, 0, bytes);
            //Console.WriteLine("Клиент: " + message);
            string response = "";
            if (message.StartsWith("List"))
            {
                Console.WriteLine(message.Split(' ')[1]);
                var files = Directory.GetFiles(message.Split(' ')[1]);
                var directories = Directory.GetDirectories(message.Split(' ')[1]);
                foreach (var file in files)
                {
                    response += file + "\n";
                }
                foreach (var file in directories)
                {
                    response += file + "\n";
                }
            }
            data = Encoding.Unicode.GetBytes(response);
            stream.Write(data, 0, data.Length);
            stream.Flush();
        }

        client.Close();
        server.Stop();
    }
}