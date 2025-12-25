namespace BestProgram.Core;

using BestProgram.Models;

/// <summary>
/// Interface for writing sensor data to output
/// </summary>
public interface IDataWriter : IDisposable
{
    Task WriteAsync(SensorData data, CancellationToken cancellationToken = default);
    Task FlushAsync();
}
