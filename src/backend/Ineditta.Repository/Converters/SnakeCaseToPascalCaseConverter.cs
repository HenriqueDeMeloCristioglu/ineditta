using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    internal sealed class SnakeCaseToPascalCaseConverter<TModel> : ValueConverter<TModel?, string?>
    {
        public SnakeCaseToPascalCaseConverter() :
            base(
                (v) => Convert(v),
                (v) => Convert(v)
                )
        {
        }

        private static string Convert(TModel? model)
        {
            return model is null ? string.Empty : JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSnakeCaseToPascalCaseNamingPolicy()
            });
        }

        private static TModel? Convert(string? json)
        {
            return string.IsNullOrEmpty(json) || json == "\"\"" || json == "''" || json == "[]" ? default : JsonSerializer.Deserialize<TModel?>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new JsonPascalCaseToSnakeCaseNamingPolicy()
            });
        }
    }
}
