using System.Diagnostics.CodeAnalysis;
using BestProgram.Configuration;
using BestProgram.Core;
using BestProgram.Infrastructure;
using BestProgram.Models;
using BestProgram.Network;
using BestProgram.Output;
using BestProgram.Parsers;
using BestProgram.Processors;

namespace BestProgram;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("===== Network Data Collector =====");
        Console.WriteLine($"Output file: {AppSettings.OutputFileName}");
        Console.WriteLine("Press Ctrl+C to stop...\n");

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            Console.WriteLine("\n\nShutting down gracefully...");
            eventArgs.Cancel = true;
            cts.Cancel();
        };

        IDataQueue dataQueue = new DataQueue(AppSettings.QueueCapacity);

        using IDataWriter fileWriter = new FileDataWriter(AppSettings.OutputFileName);
        var consumer = new DataConsumer(dataQueue, fileWriter);

        var weatherConfig = new ServerConfig(
            AppSettings.ServerHost,
            AppSettings.WeatherServerPort,
            AppSettings.WeatherPacketSize,
            "Weather"
        );
        var weatherClient = new TcpSensorClient(weatherConfig);
        var weatherParser = new WeatherDataParser();
        var weatherProducer = new DataProducer(weatherClient, weatherParser, dataQueue, "Weather");

        var coordsConfig = new ServerConfig(
            AppSettings.ServerHost,
            AppSettings.CoordinatesServerPort,
            AppSettings.CoordinatesPacketSize,
            "Coordinates"
        );
        var coordsClient = new TcpSensorClient(coordsConfig);
        var coordsParser = new CoordinatesDataParser();
        var coordsProducer = new DataProducer(coordsClient, coordsParser, dataQueue, "Coordinates");

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
            dataQueue.CompleteAdding();
            weatherClient.Dispose();
            coordsClient.Dispose();
            ((IDisposable)dataQueue).Dispose();

            Console.WriteLine("\nApplication terminated.");
        }

        Console.WriteLine("Система остановлена");
    }
}
