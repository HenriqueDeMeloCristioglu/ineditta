using System.Reflection;
using System.Text.Json.Serialization;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

using System.Text.Json;
using System.ComponentModel;

namespace Ineditta.API.Converters.JSON
{
    public class NivelEnumConverter : JsonConverter<Nivel>
    {
        public override Nivel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string enumString = reader!.GetString()!;

            foreach (Nivel enumValue in Enum.GetValues(typeToConvert))
            {
                if (GetDescription(enumValue) == enumString)
                {
                    return enumValue;
                }
            }

            throw new JsonException($"Unable to parse '{enumString}' as an enum of type '{typeToConvert.Name}'.");
        }

        public override void Write(Utf8JsonWriter writer, Nivel value, JsonSerializerOptions options)
        {
            var description = GetDescription(value);
            writer.WriteStringValue(description);
        }

        private static string? GetDescription(Nivel value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field!.GetCustomAttribute<BindAsStringAttribute>();
            var descriptionAttribute = field!.GetCustomAttribute<DescriptionAttribute>();

            if (attribute is null && descriptionAttribute is null)
            {
                return value.ToString();
            }

            return descriptionAttribute is null || string.IsNullOrEmpty(descriptionAttribute.Description) ? attribute?.ToString() : descriptionAttribute.Description;
        }
    }
}
