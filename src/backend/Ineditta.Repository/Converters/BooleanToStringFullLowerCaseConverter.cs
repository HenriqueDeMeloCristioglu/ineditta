﻿using System.Globalization;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    internal sealed class BooleanToStringFullLowerCaseConverter : ValueConverter<bool, string>
    {
        public BooleanToStringFullLowerCaseConverter() : base(v => Convert(v), v => Convert(v))
        {
        }

        private static bool Convert(string value)
        {
            return !string.IsNullOrEmpty(value) && value.ToLower(CultureInfo.InvariantCulture) == "sim";
        }

        private static string Convert(bool value)
        {
            return value ? "sim" : "não";
        }
    }
}