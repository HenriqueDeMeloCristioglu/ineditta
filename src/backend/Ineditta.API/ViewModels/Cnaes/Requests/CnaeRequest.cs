using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Cnaes.Requests
{
    public class CnaeRequest : DataTableRequest
    {
        public bool PorUsuario { get; set; }
        public bool PorGrupoDoUsuario { get; set; }
        public bool ForceGetByIds { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public IEnumerable<int>? ClientesUnidadesIds { get; set; }
        public IEnumerable<int>? EstabelecimentosIds { get; set; }
        public IEnumerable<int>? CnaesIds { get; set; }
        public IEnumerable<int>? SindicatosLaboraisId { get; set; }
        public IEnumerable<int>? SindicatosPatronaisId { get; set; }
        public bool DistinctDivisao { get; set; }
    }
}
