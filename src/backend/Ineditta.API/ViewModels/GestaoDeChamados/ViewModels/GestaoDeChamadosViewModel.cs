namespace Ineditta.API.ViewModels.GestaoDeChamados.ViewModels
{
    public class GestaoDeChamadosViewModel
    {
        public int Id { get; set; }
        public string? Tipo { get; set; }
        public string? NomeSolicitante { get; set; }
        public DateTime? InicioResposta { get; set; }
        public DateTime? DataAbertura { get; set; }
        public DateOnly? DataVencimento { get; set; }
        public string? Clausula { get; set; }
        public string? NomeResponsavel { get; set; }
        public string? Status { get; set; }
    }
}
