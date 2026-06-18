namespace TaskCleverence.Task1.Core.Interfaces
{
    /// <summary>
    /// Интерфейс для сжатия и восстановления строк
    /// </summary>
    public interface IStringCompressor
    {
        /// <summary>
        /// Сжимает входную строку
        /// </summary>
        /// <param name="input">Строка для сжатия</param>
        /// <returns>Сжатая строка</returns>
        string Compress(string input);

        /// <summary>
        /// Восстанавливает строку из сжатой строки
        /// </summary>
        /// <param name="input">Сжатая строка</param>
        /// <returns>Восстановленная строка</returns>
        string Decompress(string input);
    }
}