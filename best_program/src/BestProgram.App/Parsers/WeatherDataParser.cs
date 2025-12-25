namespace BestProgram.Parsers;

using System;
using System.Buffers.Binary;
using BestProgram.Core;
using BestProgram.Models;

/// <summary>
/// Parser for weather sensor data (temperature and pressure)
/// Packet format: 8 bytes timestamp + 4 bytes temp + 2 bytes pressure + 1 byte checksum
/// </summary>
public sealed class WeatherDataParser : IDataParser
{
    private const string SensorType = "Weather";

    public SensorData Parse(byte[] rawData)
    {
        if (rawData == null || rawData.Length != 15)
        {
            throw new ArgumentException($"Invalid data length for weather packet. Expected 15 bytes, got {rawData?.Length ?? 0}");
        }

        if (!ChecksumValidator.Validate(rawData))
        {
            throw new InvalidOperationException("Checksum validation failed");
        }

        // Parse timestamp (8 bytes, big-endian, microseconds * 10^6)
        long microseconds = BinaryPrimitives.ReadInt64BigEndian(rawData.AsSpan(0, 8));
        DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds(microseconds / 1000).DateTime;

        // Parse temperature (4 bytes, big-endian float)
        float temperature = BinaryPrimitives.ReadSingleBigEndian(rawData.AsSpan(8, 4));

        // Parse pressure (2 bytes, big-endian signed integer)
        short pressure = BinaryPrimitives.ReadInt16BigEndian(rawData.AsSpan(12, 2));

        string formattedData = $"Temperature: {temperature:F2}°C, Pressure: {pressure} hPa";

        return new SensorData(timestamp, SensorType, formattedData);
    }
}
