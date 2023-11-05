using SimpleFTP;
/*if (args.Length == 2)
{
    Environment.CurrentDirectory = args[0];
}*/
Environment.CurrentDirectory = @"C:\myFiles\programming\university\thirdSemester";
var server = new Server(7000);
await server.StartAsync();
while(true){}