using TaskCleverence.Task3.Core.Models;
using TaskCleverence.Task3.Core.Services.Parser;

namespace TaskCleverence.Task3.Tests.Parser;

/// <summary>
/// Тесты для <see cref="Format2LogParser"/> проверяют корректность парсинга лог записей формата 2
/// </summary>
public class Format2LogParserTests
{
    private readonly Format2LogParser _parser = new();

    /// <summary>
    /// Валидная строка формата 2 корректно парсится во все поля
    /// </summary>
    [Fact]
    public void TryParse_ValidLine_ReturnsTrue()
    {
        string input = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";

        bool result = _parser.TryParse(input, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal(new DateOnly(2025, 3, 10), entry.Date);
        Assert.Equal("15:14:51.5882", entry.Time);
        Assert.Equal(LogLevel.INFO, entry.Level);
        Assert.Equal("MobileComputer.GetDeviceId", entry.CallingMethod);
        Assert.Equal(" Код устройства: '@MINDEO-M40-D-410244015546'", entry.Message);
    }

    /// <summary>
    /// Строка с переставленными полями не соответствует формату возвращает false
    /// </summary>
    [Fact]
    public void TryParse_InvalidLine_ReturnsFalse()
    {
        string input = "INFORMATION|2025.03.10 15:14:51.5882|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";

        bool result = _parser.TryParse(input, out _);

        Assert.False(result);
    }

    /// <summary>
    /// Пустой CallingMethod во входной строке подставляет значение DEFAULT
    /// </summary>
    [Fact]
    public void TryParse_EmptyCallingMethod_ReturnsDefault()
    {
        string input = "2025-03-10 15:14:51.5882| INFO|11|| Код устройства: '@MINDEO-M40-D-410244015546'";

        bool result = _parser.TryParse(input, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal("DEFAULT", entry.CallingMethod);
    }

    /// <summary>
    /// Уровень логирования WARNING маппится в LogLevel.WARN
    /// </summary>
    [Fact]
    public void TryParse_MappingLevel_ReturnsWarn()
    {
        string input = "2025-03-10 15:14:51.5882| WARNING|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";

        bool result = _parser.TryParse(input, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal(LogLevel.WARN, entry.Level);
    }

    /// <summary>
    /// Неизвестный уровень логирования делает строку невалидной
    /// </summary>
    [Fact]
    public void TryParse_UnknownLevel_ReturnsFalse()
    {
        string input = "2025-03-10 15:14:51.5882| FATAL|11|MobileComputer.GetDeviceId| Critical system failure";

        bool result = _parser.TryParse(input, out var entry);

        Assert.False(result);
        Assert.Null(entry);
    }

    /// <summary>
    /// Сообщение содержащее символ pipe полностью сохраняется
    /// </summary>
    [Fact]
    public void TryParse_MessageWithPipe_PreservesFullMessage()
    {
        string input = "2025-03-10 15:14:51.5882| INFO|11|Method| Left | Right";

        bool result = _parser.TryParse(input, out var entry);

        Assert.True(result);
        Assert.NotNull(entry);
        Assert.Equal(" Left | Right", entry.Message);
    }
}
