using BestProgram.Models;

namespace BestProgram.Tests.Models;

public class SensorDataTests
{
    [Fact]
    public void Constructor_ShouldSetAllProperties()
    {
        var timestamp = DateTime.Now;
        var sensorType = "Weather";
        var formattedData = "Temperature: 25.5°C";

        var sensorData = new SensorData(timestamp, sensorType, formattedData);

        Assert.Equal(timestamp, sensorData.Timestamp);
        Assert.Equal(sensorType, sensorData.SensorType);
        Assert.Equal(formattedData, sensorData.FormattedData);
    }

    [Fact]
    public void GetFormattedTimestamp_ShouldReturnCorrectFormat()
    {
        var timestamp = new DateTime(2025, 12, 25, 14, 30, 45);
        var sensorData = new SensorData(timestamp, "Weather", "Data");

        var formatted = sensorData.GetFormattedTimestamp();

        Assert.Equal("2025-12-25 14:30:45", formatted);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var timestamp = new DateTime(2025, 12, 25, 14, 30, 45);
        var sensorData = new SensorData(timestamp, "Weather", "Temperature: 25.5°C");

        var result = sensorData.ToString();

        Assert.Equal("[2025-12-25 14:30:45] [Weather] Temperature: 25.5°C", result);
    }

    [Theory]
    [InlineData("Weather", "Temperature: 20.0°C")]
    [InlineData("Coordinates", "X: 10, Y: 20, Z: 30")]
    [InlineData("Other", "Some data")]
    public void Constructor_ShouldHandleDifferentSensorTypes(string sensorType, string data)
    {
        var timestamp = DateTime.Now;

        var sensorData = new SensorData(timestamp, sensorType, data);

        Assert.Equal(sensorType, sensorData.SensorType);
        Assert.Equal(data, sensorData.FormattedData);
    }
}
