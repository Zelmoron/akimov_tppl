using BestProgram.Models;
using BestProgram.Output;

namespace BestProgram.Tests.Output;

public class FileDataWriterAdvancedTests : IDisposable
{
    private readonly string _testFilePath = "test_output_advanced.txt";

    [Fact]
    public async Task WriteAsync_NullData_ThrowsArgumentNullException()
    {
        using var writer = new FileDataWriter(_testFilePath);

        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await writer.WriteAsync(null!);
        });
    }

    [Fact]
    public void Constructor_NullFilePath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new FileDataWriter(null!));
    }

    [Fact]
    public void Constructor_EmptyFilePath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new FileDataWriter(""));
    }

    [Fact]
    public void Constructor_WhitespaceFilePath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new FileDataWriter("   "));
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
            File.Delete(_testFilePath);
    }
}
