using BestProgram.Models;
using BestProgram.Parsers;

namespace BestProgram.Tests.Parsers;

public class CoordinatesDataParserTests
{
    private readonly CoordinatesDataParser _parser = new();

    [Fact]
    public void Parse_WithValidData_ReturnsSensorData()
    {
        var timestamp = DateTime.Now;
        var timestampMicro = (long)(timestamp.Subtract(DateTime.UnixEpoch).TotalSeconds * 1_000_000);
        var x = 100;
        var y = 200;
        var z = 300;

        var data = new byte[21];
        var timestampBytes = BitConverter.GetBytes(timestampMicro);
        if (BitConverter.IsLittleEndian) Array.Reverse(timestampBytes);
        timestampBytes.CopyTo(data, 0);
        
        var xBytes = BitConverter.GetBytes(x);
        if (BitConverter.IsLittleEndian) Array.Reverse(xBytes);
        xBytes.CopyTo(data, 8);
        
        var yBytes = BitConverter.GetBytes(y);
        if (BitConverter.IsLittleEndian) Array.Reverse(yBytes);
        yBytes.CopyTo(data, 12);
        
        var zBytes = BitConverter.GetBytes(z);
        if (BitConverter.IsLittleEndian) Array.Reverse(zBytes);
        zBytes.CopyTo(data, 16);
        
        int sum = 0;
        for (int i = 0; i < 20; i++)
            sum += data[i];
        data[20] = (byte)(sum % 256);

        var result = _parser.Parse(data);

        Assert.NotNull(result);
        Assert.Equal("Coordinates", result.SensorType);
        Assert.Contains("100", result.FormattedData);
        Assert.Contains("200", result.FormattedData);
        Assert.Contains("300", result.FormattedData);
    }

    [Fact]
    public void Parse_WithInvalidChecksum_ThrowsException()
    {
        var data = new byte[21];
        data[20] = 99;

        Assert.Throws<InvalidOperationException>(() => _parser.Parse(data));
    }

    [Fact]
    public void Parse_WithWrongSize_ThrowsException()
    {
        var data = new byte[10];

        Assert.Throws<ArgumentException>(() => _parser.Parse(data));
    }

    [Fact]
    public void Parse_WithNegativeCoordinates_ParsesCorrectly()
    {
        var timestamp = DateTime.Now;
        var timestampMicro = (long)(timestamp.Subtract(DateTime.UnixEpoch).TotalSeconds * 1_000_000);
        var x = -100;
        var y = -200;
        var z = -300;

        var data = new byte[21];
        var timestampBytes = BitConverter.GetBytes(timestampMicro);
        if (BitConverter.IsLittleEndian) Array.Reverse(timestampBytes);
        timestampBytes.CopyTo(data, 0);
        
        var xBytes = BitConverter.GetBytes(x);
        if (BitConverter.IsLittleEndian) Array.Reverse(xBytes);
        xBytes.CopyTo(data, 8);
        
        var yBytes = BitConverter.GetBytes(y);
        if (BitConverter.IsLittleEndian) Array.Reverse(yBytes);
        yBytes.CopyTo(data, 12);
        
        var zBytes = BitConverter.GetBytes(z);
        if (BitConverter.IsLittleEndian) Array.Reverse(zBytes);
        zBytes.CopyTo(data, 16);
        
        int sum = 0;
        for (int i = 0; i < 20; i++)
            sum += data[i];
        data[20] = (byte)(sum % 256);

        var result = _parser.Parse(data);

        Assert.NotNull(result);
        Assert.Contains("-100", result.FormattedData);
        Assert.Contains("-200", result.FormattedData);
        Assert.Contains("-300", result.FormattedData);
    }

    [Fact]
    public void Parse_WithZeroValues_ParsesCorrectly()
    {
        var data = new byte[21];
        
        int sum = 0;
        for (int i = 0; i < 20; i++)
            sum += data[i];
        data[20] = (byte)(sum % 256);

        var result = _parser.Parse(data);

        Assert.NotNull(result);
        Assert.Contains("0", result.FormattedData);
    }
}
