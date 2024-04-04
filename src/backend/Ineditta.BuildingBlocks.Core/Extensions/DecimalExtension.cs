using System.Globalization;

namespace Ineditta.BuildingBlocks.Core.Extensions
{
    public static class DecimalExtension
    {
#pragma warning disable S3963 // "static" fields should be initialized inline
        static DecimalExtension()
#pragma warning restore S3963 // "static" fields should be initialized inline
        {

        }
        public static string ToMoneyValue(this decimal? input)
        {
            string formatoString = string.Format(CultureInfo.GetCultureInfo("pt-br"), "R$ {0:N2}", input);

            return formatoString;
        }
    }
}
