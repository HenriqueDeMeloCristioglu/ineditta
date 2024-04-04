using System.Text.Json;

using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.BuildingBlocks.Core.JSON.Resolvers;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Ineditta.Repository.Converters
{
    internal sealed class GenericPrivateResolverCamelCaseConverter<TModel> : ValueConverter<TModel?, string?>
    {
        public GenericPrivateResolverCamelCaseConverter() : base(
            (v) => Convert(v),
            (v) => Convert(v)
            )
        {
            
        }
        
        private static string Convert(TModel? model)
        {
            return model is null ? string.Empty : System.Text.Json.JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        private static TModel? Convert(string? json)
        {
            return string.IsNullOrEmpty(json) || json == "\"\"" || json == "''" ? default :
            JsonConvert.DeserializeObject<TModel?>(json, new JsonSerializerSettings
            {
                ContractResolver = new PrivateResolver(new CamelCaseNamingStrategy()),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            });
        }
    }
}
