using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Matrizes.Requests
{
    public class MatrizRequest : DataTableRequest
    {
        public int? GrupoEconomicoId { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public bool PorUsuario { get; set; }
    }
}
