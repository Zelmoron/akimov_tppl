using BestProgram.Configuration;
using BestProgram.Core;
using BestProgram.Infrastructure;
using BestProgram.Models;
using BestProgram.Network;
using BestProgram.Output;
using BestProgram.Parsers;
using BestProgram.Processors;

Console.WriteLine("===== Network Data Collector =====");
Console.WriteLine($"Output file: {AppSettings.OutputFileName}");
Console.WriteLine("Press Ctrl+C to stop...\n");

// Create cancellation token source
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (sender, args) =>
{
    Console.WriteLine("\n\nShutting down gracefully...");
    args.Cancel = true;
    cts.Cancel();
};

// Create shared data queue
IDataQueue dataQueue = new DataQueue(AppSettings.QueueCapacity);

// Create data writer (consumer)
using IDataWriter fileWriter = new FileDataWriter(AppSettings.OutputFileName);
var consumer = new DataConsumer(dataQueue, fileWriter);

// Create weather sensor producer
var weatherConfig = new ServerConfig(
    AppSettings.ServerHost,
    AppSettings.WeatherServerPort,
    AppSettings.WeatherPacketSize,
    "Weather"
);
var weatherClient = new TcpSensorClient(weatherConfig);
var weatherParser = new WeatherDataParser();
var weatherProducer = new DataProducer(weatherClient, weatherParser, dataQueue, "Weather");

// Create coordinates sensor producer
var coordsConfig = new ServerConfig(
    AppSettings.ServerHost,
    AppSettings.CoordinatesServerPort,
    AppSettings.CoordinatesPacketSize,
    "Coordinates"
);
var coordsClient = new TcpSensorClient(coordsConfig);
var coordsParser = new CoordinatesDataParser();
var coordsProducer = new DataProducer(coordsClient, coordsParser, dataQueue, "Coordinates");

// Start all tasks
var tasks = new[]
{
    Task.Run(() => weatherProducer.RunAsync(cts.Token), cts.Token),
    Task.Run(() => coordsProducer.RunAsync(cts.Token), cts.Token),
    Task.Run(() => consumer.RunAsync(cts.Token), cts.Token)
};

try
{
    await Task.WhenAll(tasks);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operations cancelled successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Critical error: {ex.Message}");
}
finally
{
    // Cleanup
    dataQueue.CompleteAdding();
    weatherClient.Dispose();
    coordsClient.Dispose();
    ((IDisposable)dataQueue).Dispose();
    
    Console.WriteLine("\nApplication terminated.");
}

Console.WriteLine("Система остановлена");