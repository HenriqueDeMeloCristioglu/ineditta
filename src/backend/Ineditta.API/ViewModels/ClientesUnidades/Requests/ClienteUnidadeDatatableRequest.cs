using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.ClientesUnidades.Requests
{
    public class ClienteUnidadeDatatableRequest : DataTableRequest
    {
        public int? MatrizId { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public int? GrupoEconomicoId { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public bool PorUsuario { get; set; }
        public bool GrupoUsuario { get; set; }
        public bool ApenasAssociados {  get; set; }
        public int? SindicatoPatronalId { get; set; }
        public bool ForcarRetornoPorIds { get; set; }
        public IEnumerable<long>? ClientesUnidadesIds { get; set; } 
        public bool BuscarSomentePorIds { get; set; }
    }
}
