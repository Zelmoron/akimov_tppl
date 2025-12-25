using BestProgram.Models;
using BestProgram.Parsers;

namespace BestProgram.Tests.Parsers;

public class WeatherDataParserTests
{
    private readonly WeatherDataParser _parser = new();

    [Fact]
    public void Parse_WithValidData_ReturnsSensorData()
    {
        var timestamp = DateTime.Now;
        var timestampMicro = (long)(timestamp.Subtract(DateTime.UnixEpoch).TotalSeconds * 1_000_000);
        var temperature = 25.5f;
        var pressure = (short)1013;

        var data = new byte[15];
        var timestampBytes = BitConverter.GetBytes(timestampMicro);
        if (BitConverter.IsLittleEndian) Array.Reverse(timestampBytes);
        timestampBytes.CopyTo(data, 0);
        
        var tempBytes = BitConverter.GetBytes(temperature);
        if (BitConverter.IsLittleEndian) Array.Reverse(tempBytes);
        tempBytes.CopyTo(data, 8);
        
        var pressureBytes = BitConverter.GetBytes(pressure);
        if (BitConverter.IsLittleEndian) Array.Reverse(pressureBytes);
        pressureBytes.CopyTo(data, 12);
        
        int sum = 0;
        for (int i = 0; i < 14; i++)
            sum += data[i];
        data[14] = (byte)(sum % 256);

        var result = _parser.Parse(data);

        Assert.NotNull(result);
        Assert.Equal("Weather", result.SensorType);
        Assert.Contains("25.5", result.FormattedData);
        Assert.Contains("1013", result.FormattedData);
    }

    [Fact]
    public void Parse_WithInvalidChecksum_ThrowsException()
    {
        var data = new byte[15];
        data[14] = 99;

        Assert.Throws<InvalidOperationException>(() => _parser.Parse(data));
    }

    [Fact]
    public void Parse_WithWrongSize_ThrowsException()
    {
        var data = new byte[10];

        Assert.Throws<ArgumentException>(() => _parser.Parse(data));
    }

    [Fact]
    public void Parse_WithNegativeTemperature_ParsesCorrectly()
    {
        var timestamp = DateTime.Now;
        var timestampMicro = (long)(timestamp.Subtract(DateTime.UnixEpoch).TotalSeconds * 1_000_000);
        var temperature = -15.3f;
        var pressure = (short)1000;

        var data = new byte[15];
        var timestampBytes = BitConverter.GetBytes(timestampMicro);
        if (BitConverter.IsLittleEndian) Array.Reverse(timestampBytes);
        timestampBytes.CopyTo(data, 0);
        
        var tempBytes = BitConverter.GetBytes(temperature);
        if (BitConverter.IsLittleEndian) Array.Reverse(tempBytes);
        tempBytes.CopyTo(data, 8);
        
        var pressureBytes = BitConverter.GetBytes(pressure);
        if (BitConverter.IsLittleEndian) Array.Reverse(pressureBytes);
        pressureBytes.CopyTo(data, 12);
        
        int sum = 0;
        for (int i = 0; i < 14; i++)
            sum += data[i];
        data[14] = (byte)(sum % 256);

        var result = _parser.Parse(data);

        Assert.NotNull(result);
        Assert.Contains("-15.3", result.FormattedData);
    }

    [Fact]
    public void Parse_WithZeroValues_ParsesCorrectly()
    {
        var data = new byte[15];
        
        int sum = 0;
        for (int i = 0; i < 14; i++)
            sum += data[i];
        data[14] = (byte)(sum % 256);

        var result = _parser.Parse(data);

        Assert.NotNull(result);
        Assert.Contains("0.00", result.FormattedData);
    }
}
