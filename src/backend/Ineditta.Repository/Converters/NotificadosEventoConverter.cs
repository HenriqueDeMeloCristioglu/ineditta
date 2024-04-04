using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.BuildingBlocks.Core.JSON.Converters;
using Ineditta.BuildingBlocks.Core.JSON.Resolvers;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Newtonsoft.Json;

namespace Ineditta.Repository.Converters
{
    public class NotificadosEventoConverter : ValueConverter<IEnumerable<Email>, string>
    {
        public NotificadosEventoConverter() :
         base(
             (v) => Convert(v),
             (v) => Convert(v)
             )
        {
        }

        private static string Convert(IEnumerable<Email> model)
        {
            var json = model is null ? string.Empty : JsonConvert.SerializeObject(model.Select(e => e.Valor));
            return json;
        }

        private static IEnumerable<Email> Convert(string? json)
        {
            var preConvertion = string.IsNullOrEmpty(json) ? default : JsonConvert.DeserializeObject<List<string>?>(json);

            if (preConvertion is not null)
            {
                List<Email> emails = new();

                foreach (var e in preConvertion)
                {
                    var email = Email.Criar(e);
                    emails.Add(email.Value);
                }

                return emails;
            } else
            {
                return new List<Email>();
            }
        }
    }
}
