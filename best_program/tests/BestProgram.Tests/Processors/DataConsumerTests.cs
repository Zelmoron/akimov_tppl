using BestProgram.Core;
using BestProgram.Models;
using BestProgram.Processors;
using Moq;

namespace BestProgram.Tests.Processors;

public class DataConsumerTests
{
    [Fact]
    public async Task RunAsync_ConsumesAndWritesData()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();
        var sensorData = new SensorData(DateTime.Now, "Weather", "Test");

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                data = sensorData;
                return true;
            }));

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.WriteAsync(sensorData, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_EmptyQueue_DoesNotWrite()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                data = null;
                return false;
            }));

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.WriteAsync(It.IsAny<SensorData>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RunAsync_FlushesAfterDelay()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();
        var sensorData = new SensorData(DateTime.Now, "Weather", "Test");

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                data = sensorData;
                return true;
            }));

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1200);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.FlushAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_Cancellation_StopsGracefully()
    {
        var mockQueue = new Mock<IDataQueue>();
        var mockWriter = new Mock<IDataWriter>();

        mockQueue.Setup(q => q.TryDequeue(out It.Ref<SensorData?>.IsAny, It.IsAny<int>()))
            .Returns(new TryDequeueDelegate((out SensorData? data, int timeout) =>
            {
                data = null;
                return false;
            }));

        var consumer = new DataConsumer(mockQueue.Object, mockWriter.Object);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(50);

        await consumer.RunAsync(cts.Token);

        mockWriter.Verify(w => w.FlushAsync(), Times.Once);
    }

    private delegate bool TryDequeueDelegate(out SensorData? data, int timeout);
}
