namespace BestProgram.Core;

using BestProgram.Models;

/// <summary>
/// Interface for data parsing strategies
/// </summary>
public interface IDataParser
{
    SensorData Parse(byte[] rawData);
}
