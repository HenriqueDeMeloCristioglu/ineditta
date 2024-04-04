using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.Cnaes.Requests
{
    public class CnaePorListaIdsRequest : DataTableRequest
    {
        public IEnumerable<int>? CnaesIds { get; set; } = null!;
    }
}
