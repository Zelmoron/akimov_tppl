namespace BestProgram.Output;

using System;
using System.IO;
using System.Threading.Tasks;
using BestProgram.Core;
using BestProgram.Models;

/// <summary>
/// File writer for sensor data with proper timestamp formatting
/// </summary>
public sealed class FileDataWriter : IDataWriter, IDisposable
{
    private readonly StreamWriter _writer;
    private readonly object _writeLock = new();

    public FileDataWriter(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }

        _writer = new StreamWriter(filePath, append: true)
        {
            AutoFlush = false // Manually flush for better performance
        };
    }

    public async Task WriteAsync(SensorData data, CancellationToken cancellationToken = default)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        string line = data.ToString();

        lock (_writeLock)
        {
            _writer.WriteLine(line);
        }

        await Task.CompletedTask;
    }

    public async Task FlushAsync()
    {
        lock (_writeLock)
        {
            _writer.Flush();
        }

        await Task.CompletedTask;
    }

    public void Dispose()
    {
        _writer?.Flush();
        _writer?.Dispose();
    }
}
