namespace Ineditta.API.ViewModels.MapasSindicais.Requests.GerarExcel
{
    public class InformacaoAdicionalRequest
    {
        public IEnumerable<int> DocumentosIds { get; set; } = null!;
        public IEnumerable<int>? GrupoClausulaIds { get; set; }
        public IEnumerable<int>? EstruturaClausulaIds { get; set; }
        public string? PalavraChave { get; set; }
    }
}
