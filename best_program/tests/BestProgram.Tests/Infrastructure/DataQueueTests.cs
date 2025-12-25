using BestProgram.Core;
using BestProgram.Infrastructure;
using BestProgram.Models;

namespace BestProgram.Tests.Infrastructure;

public class DataQueueTests
{
    [Fact]
    public void Enqueue_ShouldAddItem()
    {
        var queue = new DataQueue(10);
        var data = new SensorData(DateTime.Now, "Weather", "Test");

        queue.Enqueue(data);
        var result = queue.Dequeue();

        Assert.Equal(data, result);
    }

    [Fact]
    public void TryDequeue_WithEmptyQueue_ReturnsFalse()
    {
        var queue = new DataQueue(10);

        var result = queue.TryDequeue(out var data, 10);

        Assert.False(result);
        Assert.Null(data);
    }

    [Fact]
    public void TryDequeue_WithData_ReturnsTrue()
    {
        var queue = new DataQueue(10);
        var sensorData = new SensorData(DateTime.Now, "Weather", "Test");
        queue.Enqueue(sensorData);

        var result = queue.TryDequeue(out var data, 100);

        Assert.True(result);
        Assert.Equal(sensorData, data);
    }

    [Fact]
    public void CompleteAdding_ShouldPreventEnqueue()
    {
        var queue = new DataQueue(10);
        queue.CompleteAdding();

        Assert.Throws<InvalidOperationException>(() => 
            queue.Enqueue(new SensorData(DateTime.Now, "Test", "Data")));
    }

    [Fact]
    public void Dispose_ShouldDisposeQueue()
    {
        var queue = new DataQueue(10);
        queue.Dispose();

        Assert.Throws<ObjectDisposedException>(() => 
            queue.Enqueue(new SensorData(DateTime.Now, "Test", "Data")));
    }

    [Fact]
    public void EnqueueDequeue_MultipleItems_MaintainsOrder()
    {
        var queue = new DataQueue(10);
        var data1 = new SensorData(DateTime.Now, "Weather", "First");
        var data2 = new SensorData(DateTime.Now, "Coordinates", "Second");
        var data3 = new SensorData(DateTime.Now, "Weather", "Third");

        queue.Enqueue(data1);
        queue.Enqueue(data2);
        queue.Enqueue(data3);

        Assert.Equal(data1, queue.Dequeue());
        Assert.Equal(data2, queue.Dequeue());
        Assert.Equal(data3, queue.Dequeue());
    }
}
