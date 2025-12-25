namespace BestProgram.Core;

using BestProgram.Models;

/// <summary>
/// Interface for data collection queue
/// </summary>
public interface IDataQueue
{
    void Enqueue(SensorData data);
    SensorData Dequeue();
    bool TryDequeue(out SensorData? data, int timeoutMs);
    int Count { get; }
    void CompleteAdding();
    bool IsCompleted { get; }
}
