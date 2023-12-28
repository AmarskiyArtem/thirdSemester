using SimpleFTP;

if (args.Length is 0 or > 2)
{
    Console.WriteLine("Port expected as first arg, working dir as second (optional)");
    return;
}

if (args.Length == 2)
{
    Environment.CurrentDirectory = args[0];
}

var server = new Server(int.Parse(args[0]));
try
{
    await server.StartAsync();
}
catch (Exception e) when (e is IOException)
{
    Console.WriteLine("Client connection error.");
}