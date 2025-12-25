namespace BestProgram.Network;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BestProgram.Configuration;
using BestProgram.Core;
using BestProgram.Models;

/// <summary>
/// TCP client for sensor data communication with auto-reconnect capability
/// </summary>
public sealed class TcpSensorClient : INetworkClient, IDisposable
{
    private readonly ServerConfig _config;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    public bool IsConnected => _client?.Connected ?? false;

    public TcpSensorClient(ServerConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            Disconnect();

            _client = new TcpClient();
            _client.ReceiveTimeout = 5000;
            _client.SendTimeout = 5000;

            await _client.ConnectAsync(_config.Host, _config.Port, cancellationToken);
            _stream = _client.GetStream();

            // Authenticate with server
            byte[] authBytes = Encoding.UTF8.GetBytes(AppSettings.AuthKey);
            await _stream.WriteAsync(authBytes, cancellationToken);

            // Read auth response (if any)
            byte[] buffer = new byte[256];
            int bytesRead = await _stream.ReadAsync(buffer, cancellationToken);
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task<byte[]?> FetchDataAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConnected || _stream == null)
        {
            return null;
        }

        try
        {
            // Send request command
            byte[] requestBytes = Encoding.UTF8.GetBytes(AppSettings.RequestCommand);
            await _stream.WriteAsync(requestBytes, cancellationToken);

            // Read response
            byte[] data = new byte[_config.PacketSize];
            int totalBytesRead = 0;

            while (totalBytesRead < _config.PacketSize)
            {
                int bytesRead = await _stream.ReadAsync(
                    data.AsMemory(totalBytesRead, _config.PacketSize - totalBytesRead),
                    cancellationToken);

                if (bytesRead == 0)
                {
                    throw new IOException("Connection closed by server");
                }

                totalBytesRead += bytesRead;
            }

            return data;
        }
        catch (Exception ex) when (ex is IOException or SocketException)
        {
            Disconnect();
            return null;
        }
    }

    public void Disconnect()
    {
        _stream?.Close();
        _stream?.Dispose();
        _stream = null;

        _client?.Close();
        _client?.Dispose();
        _client = null;
    }

    public void Dispose()
    {
        Disconnect();
        _connectionLock.Dispose();
    }
}
