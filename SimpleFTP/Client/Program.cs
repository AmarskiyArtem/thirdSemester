using FTPClient;

if (args.Length != 1)
{
    Console.WriteLine("Port expected as command line arg");
}
else
{
    var client = new Client(int.Parse(args[0]));
    await client.StartAsync();
}