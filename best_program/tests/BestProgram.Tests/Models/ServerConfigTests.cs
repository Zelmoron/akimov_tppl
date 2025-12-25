using BestProgram.Models;

namespace BestProgram.Tests.Models;

public class ServerConfigTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var host = "192.168.1.1";
        var port = 8080;
        var packetSize = 128;
        var sensorType = "Weather";

        var config = new ServerConfig(host, port, packetSize, sensorType);

        Assert.Equal(host, config.Host);
        Assert.Equal(port, config.Port);
        Assert.Equal(packetSize, config.PacketSize);
        Assert.Equal(sensorType, config.SensorType);
    }

    [Theory]
    [InlineData("95.163.237.76", 5123, 15, "Weather")]
    [InlineData("127.0.0.1", 5124, 21, "Coordinates")]
    [InlineData("localhost", 9999, 100, "Test")]
    public void Constructor_ShouldHandleDifferentValues(string host, int port, int packetSize, string sensorType)
    {
        var config = new ServerConfig(host, port, packetSize, sensorType);

        Assert.Equal(host, config.Host);
        Assert.Equal(port, config.Port);
        Assert.Equal(packetSize, config.PacketSize);
        Assert.Equal(sensorType, config.SensorType);
    }
}
