using BestProgram.Parsers;

namespace BestProgram.Tests.Parsers;

public class ChecksumValidatorTests
{
    [Fact]
    public void Validate_WithValidChecksum_ReturnsTrue()
    {
        byte[] data = { 1, 2, 3, 4, 5, 15 }; // 1+2+3+4+5=15, 15%256=15
        
        bool result = ChecksumValidator.Validate(data);
        
        Assert.True(result);
    }

    [Fact]
    public void Validate_WithInvalidChecksum_ReturnsFalse()
    {
        byte[] data = { 1, 2, 3, 4, 5, 100 }; // 1+2+3+4+5=15, 15%256=15, but checksum is 100
        
        bool result = ChecksumValidator.Validate(data);
        
        Assert.False(result);
    }

    [Fact]
    public void Validate_WithEmptyArray_ReturnsFalse()
    {
        byte[] data = Array.Empty<byte>();
        
        bool result = ChecksumValidator.Validate(data);
        
        Assert.False(result);
    }

    [Fact]
    public void Validate_WithSingleByte_ReturnsTrue()
    {
        byte[] data = { 0 }; // empty sum = 0, 0%256=0
        
        bool result = ChecksumValidator.Validate(data);
        
        Assert.True(result);
    }

    [Fact]
    public void Validate_WithMaxByteValues_ReturnsCorrectResult()
    {
        byte[] data = { 255, 255, 255, 253 }; // 255+255+255=765, 765%256=253
        
        bool result = ChecksumValidator.Validate(data);
        
        Assert.True(result);
    }

    [Theory]
    [InlineData(new byte[] { 10, 20, 30, 60 }, true)]   // 10+20+30=60
    [InlineData(new byte[] { 10, 20, 30, 50 }, false)]  // 10+20+30=60, not 50
    [InlineData(new byte[] { 0, 0, 0, 0 }, true)]       // 0+0+0=0
    public void Validate_WithVariousData_ReturnsExpectedResult(byte[] data, bool expected)
    {
        bool result = ChecksumValidator.Validate(data);
        
        Assert.Equal(expected, result);
    }
}
