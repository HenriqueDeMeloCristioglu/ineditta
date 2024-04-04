namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DocumentosSindicaisAprovadosViewModel
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public DateOnly? DataSla { get; set; }
        public string? Scrap { get; set; }
        public DateOnly? DataScrap { get; set; }
        public string? NomeUsuarioResponsavel { get; set; }
        public string? Caminho { get; set; }
    }
}
