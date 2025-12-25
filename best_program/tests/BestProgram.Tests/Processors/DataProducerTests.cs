using BestProgram.Core;
using BestProgram.Models;
using BestProgram.Processors;
using Moq;

namespace BestProgram.Tests.Processors;

public class DataProducerTests
{
    [Fact]
    public async Task RunAsync_SuccessfulFetch_EnqueuesData()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();
        
        var testData = new byte[10];
        var sensorData = new SensorData(DateTime.Now, "Test", "Data");
        
        mockClient.SetupGet(c => c.IsConnected).Returns(true);
        mockClient.Setup(c => c.FetchDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testData);
        mockParser.Setup(p => p.Parse(testData))
            .Returns(sensorData);

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(150);

        await producer.RunAsync(cts.Token);

        mockQueue.Verify(q => q.Enqueue(sensorData), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_ConnectionNeeded_Connects()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();
        
        mockClient.SetupGet(c => c.IsConnected).Returns(false);
        mockClient.Setup(c => c.ConnectAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(150);

        await producer.RunAsync(cts.Token);

        mockClient.Verify(c => c.ConnectAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_FetchReturnsNull_Reconnects()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();
        
        mockClient.SetupGet(c => c.IsConnected).Returns(true);
        mockClient.Setup(c => c.FetchDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(150);

        await producer.RunAsync(cts.Token);

        mockClient.Verify(c => c.Disconnect(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_FetchThrowsException_Reconnects()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();
        
        mockClient.SetupGet(c => c.IsConnected).Returns(true);
        mockClient.Setup(c => c.FetchDataAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("Network error"));

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(2100);

        await producer.RunAsync(cts.Token);

        mockClient.Verify(c => c.Disconnect(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_Cancellation_StopsGracefully()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();
        
        mockClient.SetupGet(c => c.IsConnected).Returns(true);
        mockClient.Setup(c => c.FetchDataAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.Cancel();

        await producer.RunAsync(cts.Token);

        // Should complete without hanging
        Assert.True(true);
    }
}
