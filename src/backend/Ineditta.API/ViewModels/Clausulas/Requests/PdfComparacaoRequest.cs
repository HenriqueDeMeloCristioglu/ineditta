namespace Ineditta.API.ViewModels.Clausulas.Requests
{
    public class PdfComparacaoRequest
    {
        public IEnumerable<PdfComparacaoModel>? DadosComparacao { get; set; }
    }

    public class PdfComparacaoModel
    {
        public string HtmlDiferencasAntigo { get; set; } = "Sem texto";
        public string HtmlDiferencasNovo { get; set; } = "Sem texto";
        public string? NomeDocumentoAntigo { get; set; }
        public string? NomeDocumentoNovo { get; set; }
        public string? PeriodoAntigo { get; set; }
        public string? PeriodoNovo { get; set; }
        public string NomeClausula { get; set; } = "Desconhecida";
        public string GrupoClausula { set; get; } = "Desconhecida";
    }
}
