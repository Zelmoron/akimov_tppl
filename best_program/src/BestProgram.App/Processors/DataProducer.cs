namespace BestProgram.Processors;

using System;
using System.Threading;
using System.Threading.Tasks;
using BestProgram.Core;
using BestProgram.Models;

/// <summary>
/// Producer that fetches data from network and parses it
/// </summary>
public sealed class DataProducer
{
    private readonly INetworkClient _client;
    private readonly IDataParser _parser;
    private readonly IDataQueue _queue;
    private readonly TimeSpan _reconnectDelay = TimeSpan.FromSeconds(2);
    private readonly string _sensorName;

    public DataProducer(INetworkClient client, IDataParser parser, IDataQueue queue, string sensorName)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _sensorName = sensorName;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"[{_sensorName}] Producer started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Ensure connection
                if (!_client.IsConnected)
                {
                    Console.WriteLine($"[{_sensorName}] Connecting...");
                    await _client.ConnectAsync(cancellationToken);
                    Console.WriteLine($"[{_sensorName}] Connected successfully");
                }

                // Fetch data
                byte[]? rawData = await _client.FetchDataAsync(cancellationToken);

                if (rawData == null)
                {
                    throw new InvalidOperationException("Failed to fetch data");
                }

                // Parse data
                SensorData parsedData = _parser.Parse(rawData);

                // Add to queue
                _queue.Enqueue(parsedData);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{_sensorName}] Error: {ex.Message}. Reconnecting...");
                _client.Disconnect();
                
                try
                {
                    await Task.Delay(_reconnectDelay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        Console.WriteLine($"[{_sensorName}] Producer stopped");
    }
}
