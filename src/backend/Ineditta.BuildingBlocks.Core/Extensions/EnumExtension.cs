using System.ComponentModel;
using System.Reflection;

namespace Ineditta.BuildingBlocks.Core.Extensions
{
    public static class EnumExtension
    {
        public static string? GetDescription<T>(this T enumValue)
           where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return string.Empty;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString()!);

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }

        public static bool TryParseToEnum<T>(this string? value, out T? enumValue) where T : struct, IConvertible
        {
            enumValue = default;

            if (!typeof(T).IsEnum)
            {
                return false;
            }
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
#pragma warning disable CS8605 // Unboxing a possibly null value.
                    if (attribute.Description == value)
                    {
                        enumValue = (T)field.GetValue(null);

                        return true;
                    }
                }
                else
                {
                    if (field.Name == value)
                    {
                        enumValue = (T)field.GetValue(null);
                        return true;
                    }
                }
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }

            return false;
        }
    }
}
