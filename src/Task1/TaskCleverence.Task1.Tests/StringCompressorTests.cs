using Microsoft.Extensions.DependencyInjection;
using TaskCleverence.Task1.Core.Interfaces;
using TaskCleverence.Task1.Core.Services;

namespace TaskCleverence.Task1.Tests;

/// <summary>
/// Тесты для <see cref="StringCompressor"/>
/// </summary>
public class StringCompressorTests
{
    private readonly IStringCompressor _compressor;

    /// <summary>
    /// Собирает DI-контейнер
    /// </summary>
    private static ServiceProvider BuildServiceProvider() => new ServiceCollection().AddSingleton<IStringCompressor, StringCompressor>().BuildServiceProvider();

    public StringCompressorTests()
    {
        _compressor = BuildServiceProvider().GetRequiredService<IStringCompressor>();
    }

    #region Compress
    /// <summary>
    /// Базовый сценарий сжатия
    /// </summary>
    [Fact]
    public void Compress_BasicInput_ReturnsCompressedString()
    {
        string input = "aaabbcccdde";
        string expected = "a3b2c3d2e";

        string result = _compressor.Compress(input);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Все символы уникальны
    /// </summary>
    [Fact]
    public void Compress_OnlyUniqueChars_ReturnsOriginalString()
    {
        string input = "abcdef";
        string expected = "abcdef";

        string result = _compressor.Compress(input);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Строка из одного символа
    /// </summary>
    [Fact]
    public void Compress_SingleChar_ReturnsChar()
    {
        string input = "a";
        string expected = "a";

        string result = _compressor.Compress(input);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Вся строка состоит из одного повторяющегося символа
    /// </summary>
    [Fact]
    public void Compress_AllSameChars_ReturnsCharWithCount()
    {
        string input = "aaaaaa";
        string expected = "a6";

        string result = _compressor.Compress(input);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Пустая строка на входе
    /// </summary>
    [Fact]
    public void Compress_EmptyString_ReturnsEmptyString()
    {
        string input = "";
        string expected = "";

        string result = _compressor.Compress(input);

        Assert.Equal(expected, result);
    }
    #endregion

    #region Decompress
    /// <summary>
    /// Базовый сценарий декомпрессии
    /// </summary>
    [Fact]
    public void Decompress_BasicInput_ReturnsDecompressedString()
    {
        string input = "a3b2c3d2e";
        string expected = "aaabbcccdde";

        string result = _compressor.Decompress(input);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Одиночный символ без цифры
    /// </summary>
    [Fact]
    public void Decompress_SingleChar_ReturnsChar()
    {
        string input = "a";
        string expected = "a";

        string result = _compressor.Decompress(input);

        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Пустая строка на входе
    /// </summary>
    [Fact]
    public void Decompress_EmptyString_ReturnsEmptyString()
    {
        string input = "";
        string expected = "";

        string result = _compressor.Decompress(input);

        Assert.Equal(expected, result);
    }
    #endregion

    /// <summary>
    /// Проверяет что сжатие и последующая декомпрессия возвращают исходную строку
    /// </summary>
    [Fact]
    public void CompressAndDecompress_RoundTrip_ReturnsOriginalString()
    {
        string original = "aaabbcccdde";

        string compressed = _compressor.Compress(original);
        string decompressed = _compressor.Decompress(compressed);

        Assert.Equal(original, decompressed);
    }
}