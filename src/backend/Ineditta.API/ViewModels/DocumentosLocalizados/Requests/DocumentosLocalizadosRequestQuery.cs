using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.DocumentosLocalizados.Requests
{
    public class DocumentosLocalizadosRequestQuery : DataTableRequest
    {
        public bool? NaoAprovados { get; set; }
    }
}
