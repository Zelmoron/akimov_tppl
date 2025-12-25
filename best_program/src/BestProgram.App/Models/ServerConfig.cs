namespace BestProgram.Models;

/// <summary>
/// Configuration for server connection
/// </summary>
public sealed class ServerConfig
{
    public string Host { get; init; }
    public int Port { get; init; }
    public int PacketSize { get; init; }
    public string SensorType { get; init; }

    public ServerConfig(string host, int port, int packetSize, string sensorType)
    {
        Host = host;
        Port = port;
        PacketSize = packetSize;
        SensorType = sensorType;
    }
}
