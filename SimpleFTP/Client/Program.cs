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
var directoryInfo = new DirectoryInfo("../../../../");
Console.WriteLine("Files:");
foreach (var file in directoryInfo.GetFiles())
{
    Console.WriteLine(file.Name); // Display only the file name
}

Console.WriteLine("Folders:");
foreach (var subDirectory in directoryInfo.GetDirectories())
{
    Console.WriteLine(subDirectory.Name); // Display only the folder name
}
