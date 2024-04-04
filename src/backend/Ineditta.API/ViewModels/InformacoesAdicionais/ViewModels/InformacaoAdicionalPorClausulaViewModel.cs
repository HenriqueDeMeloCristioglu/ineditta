namespace Ineditta.API.ViewModels.InformacoesAdicionais.ViewModels
{
    public class InformacaoAdicionalPorClausulaViewModel
    {
        public int? Id { get; set; }
        public string? NomeTipoInformacao { get; set; }
        public int EstruturaId { get; set; }
        public int TipoInformacaoId { get; set; }
    }
}
