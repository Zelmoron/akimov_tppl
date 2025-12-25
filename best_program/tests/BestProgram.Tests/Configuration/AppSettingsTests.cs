using BestProgram.Configuration;

namespace BestProgram.Tests.Configuration;

public class AppSettingsTests
{
    [Fact]
    public void ServerHost_HasCorrectValue()
    {
        Assert.Equal("95.163.237.76", AppSettings.ServerHost);
    }

    [Fact]
    public void WeatherServerPort_HasCorrectValue()
    {
        Assert.Equal(5123, AppSettings.WeatherServerPort);
    }

    [Fact]
    public void CoordinatesServerPort_HasCorrectValue()
    {
        Assert.Equal(5124, AppSettings.CoordinatesServerPort);
    }

    [Fact]
    public void WeatherPacketSize_HasCorrectValue()
    {
        Assert.Equal(15, AppSettings.WeatherPacketSize);
    }

    [Fact]
    public void CoordinatesPacketSize_HasCorrectValue()
    {
        Assert.Equal(21, AppSettings.CoordinatesPacketSize);
    }

    [Fact]
    public void AuthKey_HasCorrectValue()
    {
        Assert.Equal("isu_pt", AppSettings.AuthKey);
    }

    [Fact]
    public void RequestCommand_HasCorrectValue()
    {
        Assert.Equal("get", AppSettings.RequestCommand);
    }

    [Fact]
    public void OutputFileName_HasCorrectValue()
    {
        Assert.Equal("output.txt", AppSettings.OutputFileName);
    }

    [Fact]
    public void QueueCapacity_HasCorrectValue()
    {
        Assert.Equal(1000, AppSettings.QueueCapacity);
    }
}
