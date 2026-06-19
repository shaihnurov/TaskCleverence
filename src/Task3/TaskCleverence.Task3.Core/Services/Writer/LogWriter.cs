using TaskCleverence.Task3.Core.Interfaces;
using TaskCleverence.Task3.Core.Models;

namespace TaskCleverence.Task3.Core.Services.Writer;

/// <summary>
/// Реализация записи лог записей в выходные файлы
/// </summary>
public class LogWriter : ILogWriter
{
    /// <inheritdoc/>
    public async Task WriteAsync(IEnumerable<LogEntry> entries, string outputPath)
    {
        string? directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        try
        {
            await using var writer = new StreamWriter(outputPath, true);
            foreach (var entry in entries)
                await writer.WriteLineAsync($"{entry.Date:yyyy-MM-dd}\t{entry.Time}\t{entry.Level}\t{entry.CallingMethod}\t{entry.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write log entry: {ex.Message}");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task WriteInvalidAsync(IEnumerable<string> rawLines, string problemsPath)
    {
        string? directory = Path.GetDirectoryName(problemsPath);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        try
        {
            await using var writer = new StreamWriter(problemsPath, true);
            foreach (var rawLine in rawLines)
                await writer.WriteLineAsync(rawLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write log entry: {ex.Message}");
            throw;
        }
    }
}