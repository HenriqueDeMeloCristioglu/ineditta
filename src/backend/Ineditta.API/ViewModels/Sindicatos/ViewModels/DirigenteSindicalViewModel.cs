namespace Ineditta.API.ViewModels.Sindicatos.ViewModels
{
    public class DirigenteSindicalViewModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Cargo { get; set; }
        public string? Situacao { get; set; }
        public string? Sigla { get; set; }
        public DateOnly? InicioMandato { get; set; }
        public DateOnly? TerminoMandato { get; set; }
        public string? RazaoSocial { get; set; }
        public string? NomeUnidade { get; set; }
        public int? UnidadeId { get; set; }
        public int? SindId { get; set; }
    }
}
