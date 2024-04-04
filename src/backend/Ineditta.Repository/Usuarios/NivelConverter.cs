using System.ComponentModel;
using System.Reflection;

using Ineditta.Application.Usuarios.Entities;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Usuarios
{
    public class NivelConverter : ValueConverter<Nivel, string>
    {
        public NivelConverter() :
      base(
          (v) => Convert(v),
          (v) => Convert(v)
          )
        {
        }

        private static string Convert(Nivel model)
        {
            var fi = model.GetType().GetField(model.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes != null && attributes.Length > 0 ? attributes[0].Description : model.ToString();
            }

            return string.Empty;
        }

        private static Nivel Convert(string value)
        {
            foreach (FieldInfo field in typeof(Nivel).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == value)
#pragma warning disable CS8605 // Unboxing a possibly null value.
                        return (Nivel)field.GetValue(null);
                }
                else
                {
                    if (field.Name == value)
                        return (Nivel)field.GetValue(null);
                }
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }

            throw new ArgumentException($"No {typeof(Nivel)} with description {value} found");
        }
    }
}
