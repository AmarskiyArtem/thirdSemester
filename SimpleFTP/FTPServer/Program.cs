using FTPServer;

if (args.Length != 1)
{
    Console.WriteLine("Port expected as command line arg");
}
else
{
    var server = new Server(int.Parse(args[0]));
    var task = Task.Run(async () => await server.StartAsync());
    while (Console.ReadLine() != "exit") { }
    server.Stop();
    await task;
}