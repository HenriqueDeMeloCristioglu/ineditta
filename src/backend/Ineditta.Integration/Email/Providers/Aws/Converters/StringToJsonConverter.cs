using Newtonsoft.Json;

namespace Ineditta.Integration.Email.Providers.Aws.Converters
{
    public class StringToJsonConverter<T> : JsonConverter where T : class
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            // Deserialize the nested JSON string into MessageData
            string? json = (string?)reader.Value;

            return json is null ? default : JsonConvert.DeserializeObject<T>(json!, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            string json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            writer.WriteRawValue(json);
        }
    }
}
