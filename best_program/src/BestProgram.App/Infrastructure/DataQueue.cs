namespace BestProgram.Infrastructure;

using System.Collections.Concurrent;
using BestProgram.Core;
using BestProgram.Models;

/// <summary>
/// Thread-safe queue for sensor data using BlockingCollection
/// </summary>
public sealed class DataQueue : IDataQueue, IDisposable
{
    private readonly BlockingCollection<SensorData> _queue;

    public int Count => _queue.Count;
    public bool IsCompleted => _queue.IsCompleted;

    public DataQueue(int capacity)
    {
        _queue = new BlockingCollection<SensorData>(capacity);
    }

    public void Enqueue(SensorData data)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        _queue.Add(data);
    }

    public SensorData Dequeue()
    {
        return _queue.Take();
    }

    public bool TryDequeue(out SensorData? data, int timeoutMs)
    {
        if (_queue.TryTake(out var item, timeoutMs))
        {
            data = item;
            return true;
        }

        data = null;
        return false;
    }

    public void CompleteAdding()
    {
        _queue.CompleteAdding();
    }

    public void Dispose()
    {
        _queue.Dispose();
    }
}
