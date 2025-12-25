using BestProgram.Network;
using BestProgram.Models;

namespace BestProgram.Tests.Network;

public class TcpSensorClientAdvancedTests
{
    [Fact]
    public async Task ConnectAsync_ToRealServer_Connects()
    {
        var config = new ServerConfig("95.163.237.76", 5123, 15, "Weather");
        using var client = new TcpSensorClient(config);

        await client.ConnectAsync(CancellationToken.None);

        Assert.True(client.IsConnected);
    }

    [Fact]
    public void Disconnect_SetsIsConnectedToFalse()
    {
        var config = new ServerConfig("test.host", 8080, 100, "Test");
        using var client = new TcpSensorClient(config);

        client.Disconnect();

        Assert.False(client.IsConnected);
    }

    [Fact]
    public async Task FetchDataAsync_WhenNotConnected_ReturnsNull()
    {
        var config = new ServerConfig("test.host", 8080, 100, "Test");
        using var client = new TcpSensorClient(config);

        var result = await client.FetchDataAsync();

        Assert.Null(result);
    }

    [Fact]
    public async Task FetchDataAsync_WhenConnected_ReturnsData()
    {
        var config = new ServerConfig("95.163.237.76", 5123, 15, "Weather");
        using var client = new TcpSensorClient(config);

        await client.ConnectAsync(CancellationToken.None);
        var result = await client.FetchDataAsync();

        Assert.NotNull(result);
        Assert.Equal(15, result.Length);
    }

    [Fact]
    public async Task FetchDataAsync_MultipleRequests_ReturnsData()
    {
        var config = new ServerConfig("95.163.237.76", 5123, 15, "Weather");
        using var client = new TcpSensorClient(config);

        await client.ConnectAsync(CancellationToken.None);

        var result1 = await client.FetchDataAsync();
        var result2 = await client.FetchDataAsync();

        Assert.NotNull(result1);
        Assert.NotNull(result2);
    }

    [Fact]
    public async Task FetchDataAsync_AfterDisconnect_ReturnsNull()
    {
        var config = new ServerConfig("95.163.237.76", 5123, 15, "Weather");
        using var client = new TcpSensorClient(config);

        await client.ConnectAsync(CancellationToken.None);
        client.Disconnect();

        var result = await client.FetchDataAsync();

        Assert.Null(result);
    }

    [Fact]
    public void Constructor_NullConfig_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TcpSensorClient(null!));
    }

    [Fact]
    public async Task FetchDataAsync_CoordinatesServer_ReturnsCorrectSize()
    {
        var config = new ServerConfig("95.163.237.76", 5124, 21, "Coordinates");
        using var client = new TcpSensorClient(config);

        await client.ConnectAsync(CancellationToken.None);
        var result = await client.FetchDataAsync();

        Assert.NotNull(result);
        Assert.Equal(21, result.Length);
    }

    [Fact]
    public async Task ConnectAsync_CalledTwice_Reconnects()
    {
        var config = new ServerConfig("95.163.237.76", 5123, 15, "Weather");
        using var client = new TcpSensorClient(config);

        await client.ConnectAsync(CancellationToken.None);
        await client.ConnectAsync(CancellationToken.None);

        Assert.True(client.IsConnected);
    }
}
