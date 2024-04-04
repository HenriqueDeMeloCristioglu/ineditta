using Ineditta.BuildingBlocks.Core.JSON.Resolvers;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

namespace Ineditta.Repository.Converters
{
    public class GenericPrivateResolverConverter<TModel> : ValueConverter<TModel?, string?>
    {
        public GenericPrivateResolverConverter() :
    base(
        (v) => Convert(v),
        (v) => Convert(v)
        )
        {
        }

        private static string Convert(TModel? model)
        {
            return model is null ? string.Empty : System.Text.Json.JsonSerializer.Serialize(model);
        }

        private static TModel? Convert(string? json)
        {
            return string.IsNullOrEmpty(json) || json == "\"\"" || json == "''" ? default :
            JsonConvert.DeserializeObject<TModel?>(json, new JsonSerializerSettings
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            });
        }
    }
}
