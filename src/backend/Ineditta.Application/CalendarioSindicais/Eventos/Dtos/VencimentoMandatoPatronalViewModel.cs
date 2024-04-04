using System.Text.Json;
using System.Text.Json.Serialization;

using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class VencimentoMandatoPatronalViewModel
    {
        public int? Id { get; set; }
        public DateOnly? DataVencimento { get; set; }
        public string? SindicatoPatronal { get; set; }
        public int? SindicatoPatronalId { get; set; }
        public string? SiglaSindicato { get; set; }
        public string? Uf { get; set; }
        public string? DirigentesJson { get; set; }
        public IEnumerable<Dirigente>? Dirigentes { get; set; }
        public string? AbrangenciaJson { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<AbrangenciaMandatoSindical>? Abrangencias => string.IsNullOrEmpty(AbrangenciaJson) ? Enumerable.Empty<AbrangenciaMandatoSindical>() :
            JsonSerializer.Deserialize<IEnumerable<AbrangenciaMandatoSindical>>(AbrangenciaJson);
    }

    public class Dirigente
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? Funcao { get; set; }
    }

    public class AbrangenciaMandatoSindical
    {
        public int LocalizacaoId { get; set; }
        public string Municipio { get; set; } = null!;
        public string Uf { get; set; } = null!;
    }
}
