using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.BuildingBlocks.Core.JSON.Resolvers;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

namespace Ineditta.Repository.Documentos.Sindicais.Converters
{
    internal sealed class AbrangenciaConverter : ValueConverter<IEnumerable<Abrangencia>?, string?>
    {
        public AbrangenciaConverter() :
         base(
             (v) => Convert(v),
             (v) => Convert(v)
             )
        {
        }

        private static string? Convert(IEnumerable<Abrangencia>? model)
        {
            if (model is null)
            {
                return default;
            }

            var json = JsonConvert.SerializeObject(model);

            if (!string.IsNullOrEmpty(json))
            {
                json = json.Replace("\"Id\":", "\"id\":");
            }

            return json;
        }

        private static IEnumerable<Abrangencia>? Convert(string? json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            json = json.Replace("\"id\":", "\"Id\":");

            return JsonConvert.DeserializeObject<List<Abrangencia>?>(json, new JsonSerializerSettings
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

            });
        }
    }
}
