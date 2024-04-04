using System.Text.Json;
using System.Text.Json.Serialization;

using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class VencimentoMandatoLaboralViewModel
    {
        public int? Id { get; set; }
        public DateOnly? DataVencimento { get; set; }
        public string? SindicatoLaboral { get; set; }
        public int? SindicatoLaboralId { get; set; }
        public string? SiglaSindicato { get; set; }
        public string? Uf { get; set; }
        public string? DirigentesJson { get; set; }
        public IEnumerable<Dirigente>? Dirigentes { get; set; }
        public string? AbrangenciaJson { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<AbrangenciaMandatoSindical>? Abrangencias => string.IsNullOrEmpty(AbrangenciaJson) ? Enumerable.Empty<AbrangenciaMandatoSindical>() :
            JsonSerializer.Deserialize<IEnumerable<AbrangenciaMandatoSindical>>(AbrangenciaJson);
    }
}
