using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.GruposEconomicos.Requests
{
    public class GrupoEconomicoRequest : DataTableRequest
    {
        public bool? PorUsuario { get; set; }
    }
}
