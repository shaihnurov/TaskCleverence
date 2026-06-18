using System.Text;
using System.Text.RegularExpressions;
using TaskCleverence.Task1.Core.Interfaces;

namespace TaskCleverence.Task1.Core.Services
{
    /// <summary>
    /// Реализация сжатия строк
    /// </summary>
    public partial class StringCompressor : IStringCompressor
    {
        /// <summary>
        /// Регулярное выражение для разбора сжатой строки: захватывает символ и число повторений
        /// </summary>
        [GeneratedRegex(@"([a-zA-Z])(\d*)")]
        private static partial Regex CompressedStringRegex();

        /// <inheritdoc/>
        public string Compress(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            char[] chars = input.ToCharArray();
            StringBuilder sb = new();
            int count = 1;

            for (int i = 0; i < chars.Length - 1; i++)
            {
                if (chars[i] == chars[i + 1])
                    count++;
                else
                {
                    sb.Append(chars[i]);

                    if (count > 1)
                        sb.Append(count);

                    count = 1;
                }
            }

            if (chars.Length > 0)
            {
                sb.Append(chars[^1]);

                if (count > 1)
                    sb.Append(count);
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public string Decompress(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            MatchCollection matches = CompressedStringRegex().Matches(input);
            StringBuilder sb = new();

            foreach (Match match in matches)
            {
                char character = match.Groups[1].Value[0];
                int count = string.IsNullOrEmpty(match.Groups[2].Value) ? 1 : int.Parse(match.Groups[2].Value);

                sb.Append(new string(character, count));
            }

            return sb.ToString();
        }
    }
}