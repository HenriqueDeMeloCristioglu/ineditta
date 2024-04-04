using System.Reflection;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;

namespace Ineditta.BuildingBlocks.Core.JSON.Resolvers
{
    public class PrivateConstructorContractResolver : DefaultJsonTypeInfoResolver
    {
        public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

            if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object && jsonTypeInfo.CreateObject is null && jsonTypeInfo.Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0)
            {
                // The type doesn't have public constructors
#pragma warning disable CS8603 // Possible null reference return.
                jsonTypeInfo.CreateObject = () =>
                    Activator.CreateInstance(jsonTypeInfo.Type, true);
#pragma warning restore CS8603 // Possible null reference return.
            }

            return jsonTypeInfo;
        }
    }
}
