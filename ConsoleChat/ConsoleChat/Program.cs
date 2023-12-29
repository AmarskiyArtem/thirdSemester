using ConsoleChat;

if (args.Length == 1)
{
    var server = new Server(Int32.Parse(args[0]));
    await server.Start();
}
else if (args.Length == 2)
{
    var client = new Client(args[0], Int32.Parse(args[1]));
    await client.Start();
}
else
{
    Console.WriteLine("If you want to run application as client give host name as first arg and port as second. " +
                      "If you want to run as server give port as arg");
}