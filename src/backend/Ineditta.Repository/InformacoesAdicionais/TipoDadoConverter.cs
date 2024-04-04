using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.Clausulas.Entities.InformacoesAdicionais;
using Ineditta.Application.Usuarios.Entities;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.InformacoesAdicionais
{
    internal sealed class TipoDadoConverter : ValueConverter<TipoDado, string>
    {
        public TipoDadoConverter() :
     base(
         (v) => Convert(v),
         (v) => Convert(v)
         )
        {
        }

        private static string Convert(TipoDado model)
        {
            var fi = model.GetType().GetField(model.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                return attributes != null && attributes.Length > 0 ? attributes[0].Description : model.ToString();
            }

            return string.Empty;
        }

        private static TipoDado Convert(string value)
        {
            foreach (FieldInfo field in typeof(TipoDado).GetFields())
            {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == value)
#pragma warning disable CS8605 // Unboxing a possibly null value.
                        return (TipoDado)field.GetValue(null);
                }
                else
                {
                    if (field.Name == value)
                        return (TipoDado)field.GetValue(null);
                }
#pragma warning restore CS8605 // Unboxing a possibly null value.
            }

            throw new ArgumentException($"No {typeof(TipoDado)} with description {value} found");
        }
    }
}
