using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    public class BooleanToIntConverter : ValueConverter<bool, int>
    {
        public BooleanToIntConverter() : base(v => v ? 1 : 0, v => v == 1)
        {
        }
    }
}
