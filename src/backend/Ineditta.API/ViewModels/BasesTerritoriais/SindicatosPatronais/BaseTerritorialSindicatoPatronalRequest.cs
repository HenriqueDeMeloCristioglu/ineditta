using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.BasesTerritoriais.SindicatosPatronais
{
    public class BaseTerritorialSindicatoPatronalRequest : DataTableRequest
    {
        public BaseTerritorialSindicatoPatronalRequest()
        {
        }

        public int? SindicatoPatronalId { get; set; }
        public DateOnly? DataFinal { get; set; }
        public bool ApenasVigentes { get; set; }
    }
}
