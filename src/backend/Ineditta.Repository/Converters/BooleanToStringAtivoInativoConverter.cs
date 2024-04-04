using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    public class BooleanToStringAtivoInativoConverter : ValueConverter<bool, string?>
    {
        public BooleanToStringAtivoInativoConverter() : base(v => Convert(v), v => Convert(v))
        {
        }

        private static bool Convert(string? value)
        {
            return !string.IsNullOrEmpty(value) && value == "ativo";
        }

        private static string Convert(bool value)
        {
            return value ? "ativo" : "inativo";
        }
    }
}
