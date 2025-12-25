namespace BestProgram.Models;

/// <summary>
/// Represents data received from a sensor
/// </summary>
public sealed class SensorData
{
    public DateTime Timestamp { get; init; }
    public string SensorType { get; init; }
    public string FormattedData { get; init; }

    public SensorData(DateTime timestamp, string sensorType, string formattedData)
    {
        Timestamp = timestamp;
        SensorType = sensorType;
        FormattedData = formattedData;
    }

    /// <summary>
    /// Format timestamp as YYYY-MM-DD HH:MM:SS as per requirements
    /// </summary>
    public string GetFormattedTimestamp()
    {
        return Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public override string ToString()
    {
        return $"[{GetFormattedTimestamp()}] [{SensorType}] {FormattedData}";
    }
}
