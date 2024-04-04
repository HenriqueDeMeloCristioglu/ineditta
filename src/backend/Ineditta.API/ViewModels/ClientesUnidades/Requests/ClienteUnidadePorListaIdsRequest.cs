using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.ClientesUnidades.Requests
{
    public class ClienteUnidadePorListaIdsRequest : DataTableRequest
    {
        public IEnumerable<long>? ClienteUnidadeIds { get; set; } = null!;
    }
}
