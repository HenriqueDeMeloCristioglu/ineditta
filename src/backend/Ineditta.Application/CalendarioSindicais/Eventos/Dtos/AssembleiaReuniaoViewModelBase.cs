using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class AssembleiaReuniaoViewModelBase
    {
        public long? Id { get; set; }
        public int? AcompanhamentoCctId { get; set; }
        public string? Abrangencia { get; set; }
        public string? NomeDocumento { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public DateTime? DataHora { get; set; }
        public string? DataBase { get; set; }
        public string? FaseNegociacao { get; set; }
        public string? Observacoes { get; set; }
        public string? SindicatosLaborais { get; set; }
        public string? SindicatosPatronais { get; set; }
        public string? SindicatosLaboraisIdsString { get; set; }
        public string? SindicatosPatronaisIdsString { get; set; }
        public string? AtividadesEconomicasIdsString { get; set; }
        public string? RespostasScriptString { get; set; }
        public IEnumerable<int>? SindicatosLaboraisIds => string.IsNullOrEmpty(SindicatosLaboraisIdsString) ? Enumerable.Empty<int>() : JsonConvert.DeserializeObject<IEnumerable<int>>(SindicatosLaboraisIdsString);
        public IEnumerable<int>? SindicatosPatronaisIds => string.IsNullOrEmpty(SindicatosPatronaisIdsString) ? Enumerable.Empty<int>() : JsonConvert.DeserializeObject<IEnumerable<int>>(SindicatosPatronaisIdsString);
        public IEnumerable<int>? AtividadesEconomicasIds => string.IsNullOrEmpty(AtividadesEconomicasIdsString) ? Enumerable.Empty<int>() : JsonConvert.DeserializeObject<IEnumerable<int>>(AtividadesEconomicasIdsString);
        public IEnumerable<string>? RespostasScript => string.IsNullOrEmpty(RespostasScriptString) ? Enumerable.Empty<string>() : JsonConvert.DeserializeObject<IEnumerable<string>>(RespostasScriptString);
        public DateTime? RespostaHorario { get; set; }
    }
}
