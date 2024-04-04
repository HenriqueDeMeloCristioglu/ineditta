using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.TiposDocumentos.Requests
{
    public class TipoDocumentoRequest : DataTableRequest
    {
        public bool? Processado { get; set; }
        public IEnumerable<string>? TiposDocumentosIds { get; set; }
        public bool FiltrarSelectType { get; set; }
    }
}
