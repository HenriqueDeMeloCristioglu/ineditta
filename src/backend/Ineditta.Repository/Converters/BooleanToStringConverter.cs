using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    public class BooleanToStringConverter : ValueConverter<bool, string?>
    {
        public BooleanToStringConverter() : base(v => Convert(v), v => Convert(v))
        {
        }

        private static bool Convert(string? value)
        {
            return !string.IsNullOrEmpty(value) && value == "S";
        }

        private static string Convert(bool value)
        {
            return value ? "S" : "N";
        }
    }
}
