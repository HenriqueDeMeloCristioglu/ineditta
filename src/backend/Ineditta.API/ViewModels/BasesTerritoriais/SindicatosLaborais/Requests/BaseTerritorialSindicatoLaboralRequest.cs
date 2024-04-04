using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.BasesTerritoriais.SindicatosLaborais.Requests
{
    public class BaseTerritorialSindicatoLaboralRequest : DataTableRequest
    {
        public BaseTerritorialSindicatoLaboralRequest()
        {
        }

        public int? SindicatoLaboralId { get; set; }
        public DateOnly? DataFinal { get; set; }
        public bool ApenasVigentes { get; set; }
    }
}