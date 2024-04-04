using System.Text.Json;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ineditta.Repository.Converters
{
    internal sealed class GenericConverter<TModel> : ValueConverter<TModel?, string?>
    {
        public GenericConverter() :
            base(
                (v) => Convert(v),
                (v) => Convert(v)
                )
        {
        }

        private static string Convert(TModel? model)
        {
            return model is null ? string.Empty : JsonSerializer.Serialize(model);
        }

        private static TModel? Convert(string? json)
        {
            return string.IsNullOrEmpty(json) || json == "\"\"" || json == "''" || json == "[]" ? default : JsonSerializer.Deserialize<TModel?>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}