using BestProgram.Core;
using BestProgram.Models;
using BestProgram.Processors;
using Moq;

namespace BestProgram.Tests.Processors;

public class DataConsumerAdvancedTests
{
    [Fact]
    public async Task RunAsync_MultipleItems_WritesAll()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();
        var items = new List<SensorData>
        {
            new(DateTime.Now, "Weather", "Data1"),
            new(DateTime.Now, "Coordinates", "Data2"),
            new(DateTime.Now, "Weather", "Data3")
        };

        var index = 0;
        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                if (index < items.Count)
                {
                    data = items[index++];
                    return true;
                }
                data = null;
                return false;
            }));

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(400);

        await consumer.RunAsync(cts.Token);

        foreach (var item in items)
        {
            mockWriter.Verify(w => w.WriteAsync(item, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    [Fact]
    public async Task RunAsync_LongRunning_FlushesMultipleTimes()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                data = new SensorData(DateTime.Now, "Test", "Data");
                return true;
            }));

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(2200);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.FlushAsync(), Times.AtLeast(2));
    }

    [Fact]
    public async Task RunAsync_QueueCompleted_ExitsGracefully()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                data = null;
                return false;
            }));
        mockQueue.SetupGet(q => q.IsCompleted).Returns(true);

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1000);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.FlushAsync(), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WriteThrowsException_ContinuesProcessing()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();
        var callCount = 0;

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                callCount++;
                if (callCount <= 3)
                {
                    data = new SensorData(DateTime.Now, "Test", $"Data{callCount}");
                    return true;
                }
                data = null;
                return false;
            }));

        mockWriter.Setup(w => w.WriteAsync(It.IsAny<SensorData>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("Write error"));

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(500);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.FlushAsync(), Times.Once);
    }

    [Fact]
    public async Task RunAsync_OperationCanceledException_ExitsGracefully()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                data = new SensorData(DateTime.Now, "Test", "Data");
                return true;
            }));

        mockWriter.Setup(w => w.WriteAsync(It.IsAny<SensorData>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.FlushAsync(), Times.Once);
    }

    [Fact]
    public void Constructor_NullQueue_ThrowsArgumentNullException()
    {
        var mockWriter = new Mock<IDataWriter>();
        Assert.Throws<ArgumentNullException>(() => new DataConsumer(null!, mockWriter.Object));
    }

    [Fact]
    public void Constructor_NullWriter_ThrowsArgumentNullException()
    {
        var mockQueue = new Mock<IDataQueue>();
        Assert.Throws<ArgumentNullException>(() => new DataConsumer(mockQueue.Object, null!));
    }

    private delegate bool TryDequeueDelegate(out SensorData? data, int timeout);
}
