/*using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        string serverIP = "127.0.0.1";
        int serverPort = 7000;
        TcpClient client = new TcpClient(serverIP, serverPort);
        NetworkStream stream = client.GetStream();

        while (true)
        {
            Console.Write("Введите сообщение: ");
            string message = Console.ReadLine();
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);

            data = new byte[256];
            int bytes = stream.Read(data, 0, data.Length);
            string response = Encoding.Unicode.GetString(data, 0, bytes);
            Console.WriteLine("Сервер: " + response);
        }

        client.Close();
    }
}*/
var client = new Client.Client(7000);
await client.StartAsync();
while(true){}
