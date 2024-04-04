using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.BasesTerritoriais.SindicatosLaborais.Requests
{
    public class BaseTerritorialSindicatoLaboralPorSindicatosRequest : DataTableRequest
    {
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
    }
}
