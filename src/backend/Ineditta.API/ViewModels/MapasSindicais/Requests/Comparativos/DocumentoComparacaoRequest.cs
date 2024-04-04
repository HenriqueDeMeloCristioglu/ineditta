using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.MapasSindicais.Requests.Comparativos
{
    public class DocumentoComparacaoRequest : DataTableRequest
    {
        public IEnumerable<int>? LocalizacoesMunicipiosIds { get; set; }
        public IEnumerable<string>? LocalizacoesEstadosUfs { get; set; }
        public IEnumerable<int>? AtividadesEconomicasIds { get; set; }
        public IEnumerable<int>? NomesDocumentosIds { get; set; }
        public IEnumerable<string>? DatasBases { get; set; }
    }
}
