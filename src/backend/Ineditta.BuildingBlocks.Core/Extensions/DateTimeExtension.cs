using System.Globalization;

namespace Ineditta.BuildingBlocks.Core.Extensions
{
    public static class DateTimeExtension
    {
        public static IEnumerable<KeyValuePair<int, string>> GetAbbreviatedMonthsInYear()
        {
            for (int month = 1; month <= 12; month++)
            {
                yield return new KeyValuePair<int, string>(month, CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month).Replace(".", ""));
            }
        }
    }
}
