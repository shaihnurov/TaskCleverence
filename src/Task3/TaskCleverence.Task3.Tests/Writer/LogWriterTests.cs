using TaskCleverence.Task3.Core.Models;
using TaskCleverence.Task3.Core.Services.Writer;

namespace TaskCleverence.Task3.Tests.Writer;

/// <summary>
/// Тесты для <see cref="LogWriter"/> проверяют корректность записи в выходные файлы
/// </summary>
public class LogWriterTests : IDisposable
{
    private readonly LogWriter _writer = new();
    private readonly string _outputPath;
    private readonly string _problemsPath;

    public LogWriterTests()
    {
        _outputPath = Path.GetTempFileName();
        _problemsPath = Path.GetTempFileName();
    }

    /// <summary>
    /// Удаляем временные файлы после каждого теста
    /// </summary>
    public void Dispose()
    {
        if (File.Exists(_outputPath)) 
            File.Delete(_outputPath);

        if (File.Exists(_problemsPath)) 
            File.Delete(_problemsPath);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Запись валидной записи создаёт файл с корректно отформатированной строкой
    /// </summary>
    [Fact]
    public async Task WriteAsync_ValidEntry_WritesFormattedLine()
    {
        var entry = new LogEntry
        {
            Date = new DateOnly(2025, 3, 10),
            Time = "15:14:49.523",
            Level = LogLevel.INFO,
            CallingMethod = "DEFAULT",
            Message = "Версия программы: '3.4.0.48729'"
        };

        await _writer.WriteAsync([entry], _outputPath);

        string[] lines = await File.ReadAllLinesAsync(_outputPath);
        Assert.Single(lines);
        Assert.Equal("2025-03-10\t15:14:49.523\tINFO\tDEFAULT\tВерсия программы: '3.4.0.48729'", lines[0]);
    }

    /// <summary>
    /// Запись нескольких записей создаёт файл с соответствующим количеством строк
    /// </summary>
    [Fact]
    public async Task WriteAsync_MultipleEntries_WritesAllLines()
    {
        var entries = new List<LogEntry>
        {
            new() { Date = new DateOnly(2025, 3, 10), Time = "15:14:49.523", Level = LogLevel.INFO, CallingMethod = "DEFAULT", Message = "Сообщение 1" },
            new() { Date = new DateOnly(2025, 3, 10), Time = "15:14:50.523", Level = LogLevel.WARN, CallingMethod = "MyMethod", Message = "Сообщение 2" },
            new() { Date = new DateOnly(2025, 3, 10), Time = "15:14:51.523", Level = LogLevel.ERROR, CallingMethod = "DEFAULT", Message = "Сообщение 3" },
        };

        await _writer.WriteAsync(entries, _outputPath);

        string[] lines = await File.ReadAllLinesAsync(_outputPath);
        Assert.Equal(3, lines.Length);
    }

    /// <summary>
    /// Запись невалидной строки сохраняет её в исходном виде
    /// </summary>
    [Fact]
    public async Task WriteInvalidAsync_InvalidLine_WritesRawLine()
    {
        string rawLine = "невалидная строка лога";

        await _writer.WriteInvalidAsync([rawLine], _problemsPath);

        string[] lines = await File.ReadAllLinesAsync(_problemsPath);
        Assert.Single(lines);
        Assert.Equal(rawLine, lines[0]);
    }

    /// <summary>
    /// Повторная запись дописывает строки в конец файла, не перезаписывая его
    /// </summary>
    [Fact]
    public async Task WriteAsync_AppendMode_DoesNotOverwriteFile()
    {
        var entry = new LogEntry
        {
            Date = new DateOnly(2025, 3, 10),
            Time = "15:14:49.523",
            Level = LogLevel.INFO,
            CallingMethod = "DEFAULT",
            Message = "Сообщение"
        };

        await _writer.WriteAsync([entry], _outputPath);
        await _writer.WriteAsync([entry], _outputPath);

        string[] lines = await File.ReadAllLinesAsync(_outputPath);
        Assert.Equal(2, lines.Length);
    }
}