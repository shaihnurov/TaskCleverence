using Microsoft.Extensions.DependencyInjection;
using TaskCleverence.Task3.Core.Interfaces;
using TaskCleverence.Task3.Core.Services;
using TaskCleverence.Task3.Core.Services.Parser;
using TaskCleverence.Task3.Core.Services.Writer;

namespace TaskCleverence.Task3.Tests.Processor;

/// <summary>
/// Тесты для <see cref="LogProcessor"/> проверяют корректность работы парсинга и записи лог-файлов
/// </summary>
public class LogProcessorTests : IDisposable
{
    private readonly ILogProcessor _processor;
    private readonly string _inputPath;
    private readonly string _outputPath;
    private readonly string _problemsPath;

    public LogProcessorTests()
    {
        _processor = new ServiceCollection()
            .AddSingleton<ILogWriter, LogWriter>()
            .AddSingleton<ILogParser, Format1LogParser>()
            .AddSingleton<ILogParser, Format2LogParser>()
            .AddSingleton<ILogProcessor, LogProcessor>()
            .BuildServiceProvider()
            .GetRequiredService<ILogProcessor>();

        _inputPath = Path.GetTempFileName();
        _outputPath = Path.GetTempFileName();
        _problemsPath = Path.GetTempFileName();
    }

    /// <summary>
    /// Удаляем временные файлы после каждого теста
    /// </summary>
    public void Dispose()
    {
        if (File.Exists(_inputPath)) 
            File.Delete(_inputPath);

        if (File.Exists(_outputPath)) 
            File.Delete(_outputPath);

        if (File.Exists(_problemsPath))
            File.Delete(_problemsPath);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Валидные строки формата 1 записываются в выходной файл
    /// </summary>
    [Fact]
    public async Task ProcessAsync_ValidFormat1Lines_WritesToOutput()
    {
        await File.WriteAllLinesAsync(_inputPath, ["10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'",
            "10.03.2025 15:14:50.523 WARNING Что-то пошло не так"]);

        await _processor.ProcessAsync(_inputPath, _outputPath, _problemsPath);

        string[] lines = await File.ReadAllLinesAsync(_outputPath);
        Assert.Equal(2, lines.Length);
    }

    /// <summary>
    /// Валидные строки формата 2 записываются в выходной файл
    /// </summary>
    [Fact]
    public async Task ProcessAsync_ValidFormat2Lines_WritesToOutput()
    {
        await File.WriteAllLinesAsync(_inputPath, ["2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO'",
            "2025-03-10 15:14:52.5882| ERROR|11|MobileComputer.GetDeviceId| Ошибка подключения"]);

        await _processor.ProcessAsync(_inputPath, _outputPath, _problemsPath);

        string[] lines = await File.ReadAllLinesAsync(_outputPath);
        Assert.Equal(2, lines.Length);
    }

    /// <summary>
    /// Невалидные строки записываются в файл проблем
    /// </summary>
    [Fact]
    public async Task ProcessAsync_InvalidLines_WritesToProblems()
    {
        await File.WriteAllLinesAsync(_inputPath, ["невалидная строка", "ещё одна невалидная строка"]);

        await _processor.ProcessAsync(_inputPath, _outputPath, _problemsPath);

        string[] lines = await File.ReadAllLinesAsync(_problemsPath);
        Assert.Equal(2, lines.Length);
    }

    /// <summary>
    /// Пустые строки игнорируются и не попадают ни в output ни в problems
    /// </summary>
    [Fact]
    public async Task ProcessAsync_EmptyLines_AreIgnored()
    {
        await File.WriteAllLinesAsync(_inputPath, ["10.03.2025 15:14:49.523 INFORMATION Сообщение", "", "", "10.03.2025 15:14:50.523 ERROR Ошибка"]);

        await _processor.ProcessAsync(_inputPath, _outputPath, _problemsPath);

        string[] outputLines = await File.ReadAllLinesAsync(_outputPath);
        string[] problemLines = await File.ReadAllLinesAsync(_problemsPath);
        Assert.Equal(2, outputLines.Length);
        Assert.Empty(problemLines);
    }

    /// <summary>
    /// Смешанный файл валидные строки идут в output, невалидные в problems
    /// </summary>
    [Fact]
    public async Task ProcessAsync_MixedLines_RoutesCorrectly()
    {
        await File.WriteAllLinesAsync(_inputPath, ["10.03.2025 15:14:49.523 INFORMATION Валидная запись", "невалидная строка",
            "2025-03-10 15:14:51.5882| INFO|11|Method| Валидная запись 2"]);

        await _processor.ProcessAsync(_inputPath, _outputPath, _problemsPath);

        string[] outputLines = await File.ReadAllLinesAsync(_outputPath);
        string[] problemLines = await File.ReadAllLinesAsync(_problemsPath);
        Assert.Equal(2, outputLines.Length);
        Assert.Single(problemLines);
    }
}