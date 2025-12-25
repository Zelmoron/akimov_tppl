using BestProgram.Models;
using BestProgram.Output;

namespace BestProgram.Tests.Output;

public class FileDataWriterTests : IDisposable
{
    private readonly string _testFilePath = "test_output.txt";

    [Fact]
    public async Task WriteAsync_ShouldWriteDataToFile()
    {
        using var writer = new FileDataWriter(_testFilePath);
        var data = new SensorData(
            new DateTime(2025, 12, 25, 14, 30, 0),
            "Weather",
            "Temperature: 25.5°C, Pressure: 1013 hPa"
        );

        await writer.WriteAsync(data);
        await writer.FlushAsync();

        var content = await File.ReadAllTextAsync(_testFilePath);
        Assert.Contains("[2025-12-25 14:30:00] [Weather] Temperature: 25.5°C, Pressure: 1013 hPa", content);
    }

    [Fact]
    public async Task WriteAsync_MultipleItems_WritesAll()
    {
        using var writer = new FileDataWriter(_testFilePath);
        var data1 = new SensorData(DateTime.Now, "Weather", "Data1");
        var data2 = new SensorData(DateTime.Now, "Coordinates", "Data2");

        await writer.WriteAsync(data1);
        await writer.WriteAsync(data2);
        await writer.FlushAsync();

        var content = await File.ReadAllTextAsync(_testFilePath);
        Assert.Contains("Data1", content);
        Assert.Contains("Data2", content);
    }

    [Fact]
    public async Task FlushAsync_ShouldFlushToFile()
    {
        using var writer = new FileDataWriter(_testFilePath);
        var data = new SensorData(DateTime.Now, "Weather", "TestData");

        await writer.WriteAsync(data);
        await writer.FlushAsync();

        var content = await File.ReadAllTextAsync(_testFilePath);
        Assert.Contains("TestData", content);
    }

    [Fact]
    public async Task Dispose_ShouldCloseFile()
    {
        var writer = new FileDataWriter(_testFilePath);
        var data = new SensorData(DateTime.Now, "Weather", "Test");
        await writer.WriteAsync(data);
        writer.Dispose();

        var content = await File.ReadAllTextAsync(_testFilePath);
        Assert.Contains("Test", content);
    }

    [Fact]
    public void Constructor_CreatesFile()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);

        using var writer = new FileDataWriter(_testFilePath);

        Assert.True(File.Exists(_testFilePath));
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
}
