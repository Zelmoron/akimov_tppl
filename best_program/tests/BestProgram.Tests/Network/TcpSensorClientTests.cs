using BestProgram.Network;
using BestProgram.Models;

namespace BestProgram.Tests.Network;

public class TcpSensorClientTests
{
    [Fact]
    public void Constructor_SetsConfig()
    {
        var config = new ServerConfig("test.host", 8080, 100, "Test");

        var client = new TcpSensorClient(config);

        Assert.NotNull(client);
    }

    [Fact]
    public void Dispose_DisposesClient()
    {
        var config = new ServerConfig("test.host", 8080, 100, "Test");
        var client = new TcpSensorClient(config);

        client.Dispose();

        // Should not throw
    }

    [Fact]
    public void Disconnect_WithoutConnection_CompletesSuccessfully()
    {
        var config = new ServerConfig("test.host", 8080, 100, "Test");
        using var client = new TcpSensorClient(config);

        client.Disconnect();

        // Should not throw
    }
    
    [Fact]
    public void IsConnected_InitiallyFalse()
    {
        var config = new ServerConfig("test.host", 8080, 100, "Test");
        using var client = new TcpSensorClient(config);

        Assert.False(client.IsConnected);
    }
}
