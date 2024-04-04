namespace Ineditta.API.ViewModels.ClausulasGerais.ViewModels
{
    public class ClausulasGeraisPorDocumentoViewModel
    {
        public int Id { get; set; }
        public int? Numero { get; set; }
        public int IdDocumentoSindical { get; set; }
        public string? NomeGrupo { get; set; }
        public string? NomeClausula { get; set; }
        public DateOnly? DataProcessamento { get; set; }
        public string? NomeAprovador { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public DateOnly? DataScrap { get; set; }
        public string? NomeResponsavelProcessamento { get; set; }
        public string Resumivel { get; set; } = null!;
        public string ResumoStatus { get; set; } = null!;
    }
}
