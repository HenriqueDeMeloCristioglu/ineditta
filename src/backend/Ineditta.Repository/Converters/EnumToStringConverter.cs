using System.ComponentModel;
using System.Reflection;

using Ineditta.Application.Usuarios.Entities;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    public class EnumToStringConverter<T> : ValueConverter<T, string> where T : System.Enum
    {
        public EnumToStringConverter() :
      base(
          (v) => Convert(v),
          (v) => Convert(v)
          )
        {
        }

        private static string Convert(T model)
        {
            var fi = model.GetType().GetField(model.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes != null && attributes.Length > 0 ? attributes[0].Description : model.ToString();
            }

            return string.Empty;
        }

        private static T Convert(string value)
        {
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description?.ToLowerInvariant() == value.ToLowerInvariant())
#pragma warning disable CS8605 // Unboxing a possibly null value.
                        return (T)field.GetValue(null)!;
                }
                else
                {
                    if (field.Name.ToLowerInvariant() == value.ToLowerInvariant())
                        return (T)field.GetValue(null)!;
                }
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }

            throw new ArgumentException($"No {typeof(T)} with description {value} found");
        }
    }
}
