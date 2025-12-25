using BestProgram.Core;
using BestProgram.Models;
using BestProgram.Processors;
using Moq;

namespace BestProgram.Tests.Processors;

public class DataProducerAdvancedTests
{
    [Fact]
    public async Task RunAsync_MultipleSuccessfulFetches_EnqueuesAll()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();

        var data1 = new SensorData(DateTime.Now, "Test1", "Data1");
        var data2 = new SensorData(DateTime.Now, "Test2", "Data2");

        var fetchCount = 0;
        mockClient.SetupGet(c => c.IsConnected).Returns(true);
        mockClient.Setup(c => c.FetchDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[10]);
        mockParser.Setup(p => p.Parse(It.IsAny<byte[]>()))
            .Returns(() => fetchCount++ == 0 ? data1 : data2);

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(250);

        await producer.RunAsync(cts.Token);

        mockQueue.Verify(q => q.Enqueue(It.IsAny<SensorData>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task RunAsync_ReconnectAfterMultipleErrors_Succeeds()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();

        var attempts = 0;
        mockClient.SetupGet(c => c.IsConnected).Returns(() => attempts > 2);
        mockClient.Setup(c => c.ConnectAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
            {
                attempts++;
                return Task.CompletedTask;
            });

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(250);

        await producer.RunAsync(cts.Token);

        Assert.True(attempts >= 2);
    }

    [Fact]
    public async Task RunAsync_OperationCanceledDuringDelay_ExitsGracefully()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();

        mockClient.SetupGet(c => c.IsConnected).Returns(true);
        mockClient.Setup(c => c.FetchDataAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Test error"));

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);

        await producer.RunAsync(cts.Token);

        mockClient.Verify(c => c.Disconnect(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_OperationCanceledExceptionDuringConnect_ExitsGracefully()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();

        mockClient.SetupGet(c => c.IsConnected).Returns(false);
        mockClient.Setup(c => c.ConnectAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();

        await producer.RunAsync(cts.Token);

        Assert.True(true);
    }

    [Fact]
    public void Constructor_NullClient_ThrowsArgumentNullException()
    {
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();

        Assert.Throws<ArgumentNullException>(() =>
            new DataProducer(null!, mockParser.Object, mockQueue.Object, "Test"));
    }

    [Fact]
    public void Constructor_NullParser_ThrowsArgumentNullException()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockQueue = new Mock<IDataQueue>();

        Assert.Throws<ArgumentNullException>(() =>
            new DataProducer(mockClient.Object, null!, mockQueue.Object, "Test"));
    }

    [Fact]
    public void Constructor_NullQueue_ThrowsArgumentNullException()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();

        Assert.Throws<ArgumentNullException>(() =>
            new DataProducer(mockClient.Object, mockParser.Object, null!, "Test"));
    }

    [Fact]
    public async Task RunAsync_ParserThrowsException_Reconnects()
    {
        var mockClient = new Mock<INetworkClient>();
        var mockParser = new Mock<IDataParser>();
        var mockQueue = new Mock<IDataQueue>();

        mockClient.SetupGet(c => c.IsConnected).Returns(true);
        mockClient.Setup(c => c.FetchDataAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new byte[10]);
        mockParser.Setup(p => p.Parse(It.IsAny<byte[]>()))
            .Throws(new ArgumentException("Invalid data"));

        var producer = new DataProducer(mockClient.Object, mockParser.Object, mockQueue.Object, "TestSensor");
        var cts = new CancellationTokenSource();
        cts.CancelAfter(300);

        await producer.RunAsync(cts.Token);

        mockClient.Verify(c => c.Disconnect(), Times.AtLeastOnce);
    }
}
