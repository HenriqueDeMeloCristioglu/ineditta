using Newtonsoft.Json;

namespace Ineditta.BuildingBlocks.Core.JSON.Converters
{
    public class BooleanToIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            return reader?.Value?.ToString() == "1";
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is bool booleanValue)
            {
                writer.WriteValue(booleanValue ? "1" : "0");
            }
        }
    }
}
