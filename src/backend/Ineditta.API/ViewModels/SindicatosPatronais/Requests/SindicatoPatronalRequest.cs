using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.SindicatosPatronais.Requests
{
    public class SindicatoPatronalRequest : DataTableRequest
    {
        public bool PorUsuario { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? ClientesUnidadesIds { get; set; }
        public IEnumerable<int>? LocalizacoesIds { get; set; }
        public IEnumerable<string>? Ufs { get; set; }
        public IEnumerable<string>? Regioes { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
    }
}