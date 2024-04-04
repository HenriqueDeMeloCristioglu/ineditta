namespace Ineditta.API.ViewModels.InformacoesAdicionais.ViewModels
{
    public class InformacaoAdicionalDadoViewModel
    {
        public int Id { get; set; }
        public string? SequenciaLinha { get; set; }
        public int? SequenciaItem { get; set; }
        public int? GrupoId { get; set; }
        public int? EstruturaId { get; set; }
        public int? DocumentoId { get; set; }
        public int? TipoInfomacaoAdicionalId { get; set; }
        public int? Nome { get; set; }
        public string? Texto { get; set; }
        public string? Combo { get; set; }
        public string? Hora { get; set; }
        public decimal? Percentual { get; set; }
        public string? Data { get; set; }
        public string? Descricao { get; set; }
        public decimal? Numerico { get; set; }
    }
}
