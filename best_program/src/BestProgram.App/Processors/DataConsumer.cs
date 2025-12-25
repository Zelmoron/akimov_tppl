namespace BestProgram.Processors;

using System;
using System.Threading;
using System.Threading.Tasks;
using BestProgram.Core;
using BestProgram.Models;

/// <summary>
/// Consumer that reads data from queue and writes to output
/// </summary>
public sealed class DataConsumer
{
    private readonly IDataQueue _queue;
    private readonly IDataWriter _writer;
    private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(1);

    public DataConsumer(IDataQueue queue, IDataWriter writer)
    {
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[Consumer] Started");
        
        DateTime lastFlush = DateTime.UtcNow;
        int itemsProcessed = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Try to dequeue with timeout
                if (_queue.TryDequeue(out var data, timeoutMs: 100))
                {
                    await _writer.WriteAsync(data!, cancellationToken);
                    itemsProcessed++;

                    // Periodic flush for reliability
                    if (DateTime.UtcNow - lastFlush > _flushInterval)
                    {
                        await _writer.FlushAsync();
                        lastFlush = DateTime.UtcNow;
                    }
                }
                else if (_queue.IsCompleted)
                {
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Consumer] Error writing data: {ex.Message}");
            }
        }

        // Final flush
        await _writer.FlushAsync();
        Console.WriteLine($"[Consumer] Stopped. Processed {itemsProcessed} items");
    }
}
