using Microsoft.Extensions.DependencyInjection;
using TaskCleverence.Task3.Core.Interfaces;
using TaskCleverence.Task3.Core.Services;
using TaskCleverence.Task3.Core.Services.Parser;
using TaskCleverence.Task3.Core.Services.Writer;

await RunAsync();

internal partial class Program
{
    public static async Task RunAsync()
    {
        var provider = BuildServiceProvider();
        var processor = provider.GetRequiredService<ILogProcessor>();

        string inputPath = Input("Enter input log file path");
        string outputPath = Input("Enter output file path");
        string problemsPath = Input("Enter problems file path");

        try
        {
            Console.WriteLine("\nProcessing...");
            await processor.ProcessAsync(inputPath, outputPath, problemsPath);
            Console.WriteLine("Log processing completed successfully");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: Input file not found: {inputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static string Input(string message)
    {
        string? input;

        do
        {
            Console.Write($"{message}: ");
            input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
                Console.WriteLine("Path cannot be empty, please try again");

        } while (string.IsNullOrWhiteSpace(input));

        return input;
    }

    private static ServiceProvider BuildServiceProvider() 
        => new ServiceCollection()
            .AddSingleton<ILogWriter, LogWriter>()
            .AddSingleton<ILogParser, Format1LogParser>()
            .AddSingleton<ILogParser, Format2LogParser>()
            .AddSingleton<ILogProcessor, LogProcessor>()
            .BuildServiceProvider();
}