using BestProgram.Infrastructure;
using BestProgram.Models;

namespace BestProgram.Tests.Infrastructure;

public class DataQueueAdvancedTests
{
    [Fact]
    public void Count_ReturnsCorrectValue()
    {
        var queue = new DataQueue(10);
        queue.Enqueue(new SensorData(DateTime.Now, "Test", "Data1"));
        queue.Enqueue(new SensorData(DateTime.Now, "Test", "Data2"));

        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void IsCompleted_InitiallyFalse()
    {
        var queue = new DataQueue(10);

        Assert.False(queue.IsCompleted);
    }

    [Fact]
    public void IsCompleted_AfterCompleteAdding_True()
    {
        var queue = new DataQueue(10);
        queue.CompleteAdding();

        Assert.True(queue.IsCompleted);
    }

    [Fact]
    public void Enqueue_Null_ThrowsArgumentNullException()
    {
        var queue = new DataQueue(10);

        Assert.Throws<ArgumentNullException>(() => queue.Enqueue(null!));
    }

    [Fact]
    public void TryDequeue_AfterCompleteAdding_ReturnsFalse()
    {
        var queue = new DataQueue(10);
        queue.CompleteAdding();

        var result = queue.TryDequeue(out var data, 100);

        Assert.False(result);
    }
}
