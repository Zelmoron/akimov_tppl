namespace BestProgram.Configuration;

/// <summary>
/// Application configuration constants
/// </summary>
public static class AppSettings
{
    public const string ServerHost = "95.163.237.76";
    public const int WeatherServerPort = 5123;
    public const int CoordinatesServerPort = 5124;
    
    public const int WeatherPacketSize = 15; // 8 + 4 + 2 + 1
    public const int CoordinatesPacketSize = 21; // 8 + 4 + 4 + 4 + 1
    
    public const string AuthKey = "isu_pt";
    public const string RequestCommand = "get";
    
    public const string OutputFileName = "output.txt";
    public const int QueueCapacity = 1000;
}
