namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DocumentoSindicalClausulasDataTabelViewModel
    {
        public int Id { get; set; }
        public required string Grupo { get; set;}
        public required string Nome { get; set; }
        public string? Texto { get; set; }
        public DateOnly? DataProcessamento { get; set; }
        public string? ProcessadoPor { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public string? AprovadoPor { get; set; }
    }
}
