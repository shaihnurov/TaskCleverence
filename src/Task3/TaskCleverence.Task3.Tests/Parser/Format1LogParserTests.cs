using TaskCleverence.Task3.Core.Models;
using TaskCleverence.Task3.Core.Services.Parser;

namespace TaskCleverence.Task3.Tests.Parser;

/// <summary>
/// Тесты для <see cref="Format1LogParser"/> проверяют корректность парсинга лог записей формата 1
/// </summary>
public class Format1LogParserTests
{
    private readonly Format1LogParser _parser = new();

    /// <summary>
    /// Валидная строка формата 1 корректно парсится во все поля
    /// </summary>
    [Fact]
    public void TryParse_ValidLine_ReturnsTrue()
    {
        string input = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";

        bool result = _parser.TryParse(input, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal(new DateOnly(2025, 3, 10), entry.Date);
        Assert.Equal("15:14:49.523", entry.Time);
        Assert.Equal(LogLevel.INFO, entry.Level);
        Assert.Equal("DEFAULT", entry.CallingMethod);
        Assert.Equal("Версия программы: '3.4.0.48729'", entry.Message);
    }

    /// <summary>
    /// Строка с неверным форматом даты не соответствует паттерну возвращает false
    /// </summary>
    [Fact]
    public void TryParse_InvalidLine_ReturnsFalse()
    {
        string input = "10-03-2025 15:14:49.523 INFO Версия программы: '3.4.0.48729'";

        bool result = _parser.TryParse(input, out _);

        Assert.False(result);
    }

    /// <summary>
    /// Уровень логирования WARNING маппится в LogLevel.WARN
    /// </summary>
    [Fact]
    public void TryParse_MappingLevel_ReturnsWarn()
    {
        string input = "10.03.2025 15:14:49.523 WARNING Версия программы: '3.4.0.48729'";

        bool result = _parser.TryParse(input, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal(LogLevel.WARN, entry.Level);
    }

    /// <summary>
    /// Уровень логирования ERROR маппится в LogLevel.ERROR
    /// </summary>
    [Fact]
    public void TryParse_MappingLevel_ReturnsError()
    {
        string input = "10.03.2025 15:14:49.523 ERROR Версия программы: '3.4.0.48729'";

        bool result = _parser.TryParse(input, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal(LogLevel.ERROR, entry.Level);
    }

    /// <summary>
    /// Неизвестный уровень логирования делает строку невалидной
    /// </summary>
    [Fact]
    public void TryParse_UnknownLevel_ReturnsFalse()
    {
        string input = "10.03.2025 15:14:49.523 FATAL Critical system failure";

        bool result = _parser.TryParse(input, out var entry);

        Assert.False(result);
        Assert.Null(entry);
    }
}
