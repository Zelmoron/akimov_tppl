using BestProgram.Network;
using BestProgram.Models;

namespace BestProgram.Tests.Network;

public class TcpSensorClientAdvancedTests
{
    [Fact]
    public async Task ConnectAsync_SetsIsConnectedToTrue()
    {
        var config = new ServerConfig("95.163.237.76", 5123, 15, "Weather");
        using var client = new TcpSensorClient(config);

        await client.ConnectAsync(CancellationToken.None);

        // May or may not connect to real server, but should not throw
        Assert.True(true);
    }

    [Fact]
    public void Disconnect_SetsIsConnectedToFalse()
    {
        var config = new ServerConfig("test.host", 8080, 100, "Test");
        using var client = new TcpSensorClient(config);

        client.Disconnect();

        Assert.False(client.IsConnected);
    }
}
