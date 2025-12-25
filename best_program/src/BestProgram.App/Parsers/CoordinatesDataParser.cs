namespace BestProgram.Parsers;

using System;
using System.Buffers.Binary;
using BestProgram.Core;
using BestProgram.Models;

/// <summary>
/// Parser for coordinates sensor data (X, Y, Z values)
/// Packet format: 8 bytes timestamp + 4 bytes X + 4 bytes Y + 4 bytes Z + 1 byte checksum
/// </summary>
public sealed class CoordinatesDataParser : IDataParser
{
    private const string SensorType = "Coordinates";

    public SensorData Parse(byte[] rawData)
    {
        if (rawData == null || rawData.Length != 21)
        {
            throw new ArgumentException($"Invalid data length for coordinates packet. Expected 21 bytes, got {rawData?.Length ?? 0}");
        }

        if (!ChecksumValidator.Validate(rawData))
        {
            throw new InvalidOperationException("Checksum validation failed");
        }

        // Parse timestamp (8 bytes, big-endian, microseconds * 10^6)
        long microseconds = BinaryPrimitives.ReadInt64BigEndian(rawData.AsSpan(0, 8));
        DateTime timestamp = DateTimeOffset.FromUnixTimeMilliseconds(microseconds / 1000).DateTime;

        // Parse X coordinate (4 bytes, big-endian signed integer)
        int x = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(8, 4));

        // Parse Y coordinate (4 bytes, big-endian signed integer)
        int y = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(12, 4));

        // Parse Z coordinate (4 bytes, big-endian signed integer)
        int z = BinaryPrimitives.ReadInt32BigEndian(rawData.AsSpan(16, 4));

        string formattedData = $"X: {x}, Y: {y}, Z: {z}";

        return new SensorData(timestamp, SensorType, formattedData);
    }
}
