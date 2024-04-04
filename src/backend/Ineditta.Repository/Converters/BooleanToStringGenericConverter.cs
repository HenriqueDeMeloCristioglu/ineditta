using System.Globalization;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    public class BooleanToStringGenericConverter : ValueConverter<bool, string?>
    {

        public BooleanToStringGenericConverter(string trueValueString, string falseValueString) : base(v => Convert(v, trueValueString, falseValueString), v => Convert(v, trueValueString))
        {
        }

        private static bool Convert(string? value, string trueValueString)
        {
            return !string.IsNullOrEmpty(value) && value == trueValueString.ToLower(CultureInfo.InvariantCulture);
        }

        private static string Convert(bool value, string trueValueString, string falseValueString)
        {
            return value ? trueValueString.ToLower(CultureInfo.InvariantCulture) : falseValueString.ToLower(CultureInfo.InvariantCulture);
        }
    }
}
