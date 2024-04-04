using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.ClientesUnidades.Requests
{
    public class InformacoesEstabelecimentosRequest : DataTableRequest
    {
        public IEnumerable<int>? UnidadesIds { get; set; }
    }
}
