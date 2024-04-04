using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.DocumentosSindicais.Requests
{
    public class DocumentoSincicalRequestQuery : DataTableRequest
    {
        public bool? Processados { get; set; }
        public string? Descricao { get; set; }
    }
}
