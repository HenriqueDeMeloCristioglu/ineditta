using System.Globalization;
using System.Web;

namespace Ineditta.BuildingBlocks.Core.Web.API.Extensions
{
    public static class QueryStringExtension
    {
        public static string ToQueryString(object obj, bool camelCase = false)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select (camelCase ? ToCamelCase(p.Name) : p.Name) + "=" + HttpUtility.UrlEncode(p?.GetValue(obj, null)?.ToString());

            return string.Join("&", properties.ToArray());
        }
        
        private static string ToCamelCase(string str)
        {
            return !string.IsNullOrEmpty(str) && str.Length > 1
                ? char.ToLower(str[0], CultureInfo.InvariantCulture) + str[1..]
                : str?.ToLower(CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return string.Concat(input.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower(CultureInfo.InvariantCulture);
        }
    }
}
