using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class EventoClausulaViewModel
    {
        public long? Id { get; set; }
        public int? DocId { get; set; }
        [JsonIgnore]
        public string? Abrangencia { get; set; }
        public string? NomeCampo { get; set; }
        public string? NomeDocumento { get; set; }
        public string? SiglaDocumento { get; set; }
        public string? NomeInfoAdicional { get; set; }
        public int? ClausulaGeralEstruturaClausulaId { get; set; }
        [JsonIgnore]
        public string? AtividadesEconomicas { get; set; }
        public DateOnly? DataVencimento { get; set; }

        [JsonIgnore]
        public string? SindicatoLaboral { get; set; }
        [JsonIgnore]
        public string? SindicatoPatronal { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<AbrangenciaDoc>? Abrangencias => string.IsNullOrEmpty(Abrangencia) ? Enumerable.Empty<AbrangenciaDoc>() :
            JsonSerializer.Deserialize<IEnumerable<AbrangenciaDoc>>(Abrangencia);

        [NotSearchableDataTable]
        public IEnumerable<Sind>? SindicatosLaborais => string.IsNullOrEmpty(SindicatoLaboral) ? Enumerable.Empty<Sind>() :
            JsonSerializer.Deserialize<IEnumerable<Sind>>(SindicatoLaboral);

        [NotSearchableDataTable]
        public IEnumerable<Sind>? SindicatosPatronais => string.IsNullOrEmpty(SindicatoPatronal) ? Enumerable.Empty<Sind>() :
            JsonSerializer.Deserialize<IEnumerable<Sind>>(SindicatoPatronal);

        [NotSearchableDataTable]
        public IEnumerable<CnaeDoc>? Cnaes
        {
            get
            {
                if (string.IsNullOrEmpty(AtividadesEconomicas))
                {
                    return Enumerable.Empty<CnaeDoc>();
                }
                var teste = JsonSerializer.Deserialize<IEnumerable<object>>(AtividadesEconomicas);
                Console.WriteLine(teste);

                var result = JsonSerializer.Deserialize<IEnumerable<CnaeDoc>>(AtividadesEconomicas);
                return result;
            }
        }

        [NotSearchableDataTable]
        public int TipoEventoId { get; set; }

        [NotSearchableDataTable]
        public int SubtipoEventoId { get; set; }
    }
}
