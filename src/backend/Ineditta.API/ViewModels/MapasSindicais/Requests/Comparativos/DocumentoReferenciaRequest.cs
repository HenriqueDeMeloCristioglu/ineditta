using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.MapasSindicais.Requests.Comparativos
{
    public class DocumentoReferenciaRequest : DataTableRequest
    {
        public IEnumerable<int>? LocalizacoesMunicipiosIds { get; set; }
        public IEnumerable<string>? LocalizacoesEstadosUfs { get; set; }
        public IEnumerable<int>? AtividadesEconomicasIds { get; set; }
        public IEnumerable<int>? NomeDocumentoIds { get; set; }
        public IEnumerable<string>? DatasBases { get; set; }
        public IEnumerable<int>? EstabelecimentosIds { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? MatrizesIds { get; set; }
        public int? IgnorarDocumentoId { get; set; }
    }
}
