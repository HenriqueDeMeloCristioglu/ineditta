namespace Ineditta.API.ViewModels.InformacoesAdicionais.ViewModels
{
    public class InformacaoAdicionalCamposViewModel
    {
        public int Id { get; set; }
        public int EstruturaId { get; set; }
        public int CdInformacaoId { get; set; }
        public string? TipoDadadoId { get; set; }
        public string? TipoInformacaoNome { get; set; }
        public InformacaoAdicionalCombosViewModel? Combo { get; set; }
    }
}
