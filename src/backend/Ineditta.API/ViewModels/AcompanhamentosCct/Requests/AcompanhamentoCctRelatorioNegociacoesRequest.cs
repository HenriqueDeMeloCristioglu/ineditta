using Ineditta.BuildingBlocks.Core.Web.API.Datatables;

namespace Ineditta.API.ViewModels.AcompanhamentosCct.Requests
{
    public class AcompanhamentoCctRelatorioNegociacoesRequest : DataTableRequest
    {
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? EmpresasIds { get; set; }
        public IEnumerable<int>? EstabelecimentosIds { get; set; }
        public IEnumerable<string>? AtividadesEconomicasIds { get; set; }
        public IEnumerable<string>? Localizacoes { get; set; }
        public string? TipoLocalizacao { get; set; }
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
        public IEnumerable<string>? DatasBases { get; set; }
        public IEnumerable<int>? NomeDocumento { get; set; }
        public IEnumerable<string>? Fases { get; set; }
        public DateOnly? DataProcessamentoInicial { get; set; }
        public DateOnly? DataProcessamentoFinal { get; set; }
    }
}
