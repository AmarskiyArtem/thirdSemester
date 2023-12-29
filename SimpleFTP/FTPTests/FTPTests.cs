using System.Text;

namespace FTPTests;

public class Tests
{
    private Server _server;
    private Client _client;
    private readonly int _port = 7000;

    [SetUp]
    public async Task Setup()
    {
        _server = new Server(_port);
        _client = new Client(_port);

        _ = Task.Run(() => _server.StartAsync());
        await Task.Delay(100);
    }

    [TearDown]
    public void TearDown()
    {
        _server.Stop();
        Task.Delay(100);
    }
    
    [Test]
    public async Task ListInvalidPathShouldMinusOne()
        => Assert.That(await _client.ListAsync("/InvalidPath"), Is.EqualTo("-1"));
    
    [Test]
    public async Task GetInvalidPathShouldMinusOne()
        => Assert.That(await _client.GetAsync("/InvalidPath"), Is.EqualTo("-1"));
    
    [Test]
    public async Task ListValidDirShouldCorrectAnswer()
        => Assert.That(await _client.ListAsync("../../../TestFolder/"), 
            Is.EqualTo("2 Fold True zero.txt False"));

    [Test]
    public async Task GetValidDirShouldCorrectAnswer()
    {
        var response = await _client.GetAsync("../../../TestFolder/zero.txt");
        if (response is null)
        {
            Assert.Fail();
            return;
        }

        var parts = response.Split(' ');

        var actualCount = parts[0];
        var actualContent = Encoding.UTF8.GetString(Enumerable.Range(0, parts[1].Length)
                                                 .Where(x => x % 2 == 0)
                                                 .Select(x => Convert.ToByte(parts[1].Substring(x, 2), 16))
                                                 .ToArray());

        var expectedCount = "1";
        var expectedContent = "0";

        Assert.Multiple(() =>
        {
            Assert.That(actualCount, Is.EqualTo(expectedCount));
            Assert.That(actualContent, Is.EqualTo(expectedContent));
        });
    }
    
}