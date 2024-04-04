using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.JSON.Converters;
using Ineditta.BuildingBlocks.Core.JSON.Resolvers;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

namespace Ineditta.Repository.Usuarios
{
    public class UsuarioModuloConverter : ValueConverter<IEnumerable<UsuarioModulo>?, string>
    {
        public UsuarioModuloConverter() :
         base(
             (v) => Convert(v),
             (v) => Convert(v)
             )
        {
        }

        private static string Convert(IEnumerable<UsuarioModulo>? model)
        {
            var json = model is null ? string.Empty : JsonConvert.SerializeObject(model, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new BooleanToIntConverter()
                }
            });

            if (!string.IsNullOrEmpty(json))
            {
                json = json.Replace("\"Id\":", "\"id\":");
            }

            return json;
        }

        private static IEnumerable<UsuarioModulo>? Convert(string? json)
        {
            return string.IsNullOrEmpty(json) ? default : JsonConvert.DeserializeObject<List<UsuarioModulo>?>(json, new JsonSerializerSettings
            {
                ContractResolver = new PrivateResolver(),
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                Converters = new List<JsonConverter>
                {
                    new BooleanToIntConverter()
                }

            });
        }
    }
}
