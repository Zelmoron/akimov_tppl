namespace BestProgram.Core;

using BestProgram.Models;

/// <summary>
/// Interface for network communication with sensors
/// </summary>
public interface INetworkClient
{
    Task<byte[]?> FetchDataAsync(CancellationToken cancellationToken = default);
    Task ConnectAsync(CancellationToken cancellationToken = default);
    void Disconnect();
    bool IsConnected { get; }
}
