namespace BestProgram.Parsers;

using System;

/// <summary>
/// Utility for checksum validation
/// </summary>
public static class ChecksumValidator
{
    /// <summary>
    /// Validates checksum (sum of all bytes mod 256)
    /// </summary>
    public static bool Validate(byte[] data)
    {
        if (data == null || data.Length == 0)
        {
            return false;
        }

        int sum = 0;
        for (int i = 0; i < data.Length - 1; i++)
        {
            sum += data[i];
        }

        byte checksum = (byte)(sum % 256);
        return checksum == data[^1];
    }
}
