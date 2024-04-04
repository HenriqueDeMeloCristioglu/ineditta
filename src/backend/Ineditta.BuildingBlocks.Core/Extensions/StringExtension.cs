using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ineditta.BuildingBlocks.Core.Extensions
{
    public static class StringExtension
    {
        private static readonly Dictionary<string, int> Months;

#pragma warning disable S3963 // "static" fields should be initialized inline
        static StringExtension()
#pragma warning restore S3963 // "static" fields should be initialized inline
        {
            var cultureInfo = new CultureInfo("pt-br");

            Months = new Dictionary<string, int>();

            for (int i = 1; i < cultureInfo.DateTimeFormat.AbbreviatedMonthNames.Length; i++)
            {
                Months.Add(cultureInfo.DateTimeFormat.AbbreviatedMonthNames[i - 1].Replace(".", ""), i);
            }
        }
        
        public static string ToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var textInfo = CultureInfo.CurrentCulture.TextInfo;

            return string.Concat(
                input.ToLower(CultureInfo.InvariantCulture)
                    .Split(new char[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(word => textInfo.ToTitleCase(word))
                    .Select(word => new string(word.Where(char.IsLetterOrDigit).ToArray()))
                );
        }

        public static string OnlyNumbers(this string input)
        {
            string pattern = @"\D";
            string formatInput = Regex.Replace(input, pattern, "");

            return formatInput;
        }

        public static string? JoinToString(this IEnumerable<string> input, string separator)
        {
            return input is null || !input.Any() || string.IsNullOrEmpty(separator) ? default : "[" + string.Join(separator, input.Select(i => $"\"{i}\"")) + "]";
        }

        public static string? JoinToInt(this IEnumerable<int> input, string separator)
        {
            return input is null || !input.Any() || string.IsNullOrEmpty(separator) ? default : "[" + string.Join(separator, input.Select(i => $"{i}")) + "]";
        }
        public static IEnumerable<string> SplitToString(this string input, string separator)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(separator))
                return Enumerable.Empty<string>();

            input = input.Trim('[', ']'); // Remove os colchetes da string, se presentes
            string[] parts = input.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            List<string> result = new List<string>();
            foreach (string part in parts)
            {
                if (part.StartsWith('"') && part.EndsWith('"'))
                {
                    result.Add(part.Trim('\"'));
                }
            }

            return result;
        }

        public static IEnumerable<int> SplitToInt(this string input, string separator)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(separator))
                yield break;

            input = input.Trim('[', ']');
            string[] parts = input.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int intValue))
                {
                    yield return intValue;
                }
            }
        }

        public static int GetMonthNumberByAbbreviatedMonthName(this string input)
        {
            return string.IsNullOrEmpty(input) || !Months.TryGetValue(input.ToLowerInvariant(), out int monthNumber) ?
                default :
                monthNumber;
        }

        public static string RemoveAccents(this string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormKD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
