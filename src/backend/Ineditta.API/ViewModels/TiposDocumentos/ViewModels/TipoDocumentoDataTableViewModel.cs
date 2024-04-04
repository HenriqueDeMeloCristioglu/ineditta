namespace Ineditta.API.ViewModels.TiposDocumentos.ViewModels
{
    public class TipoDocumentoDataTableViewModel
    {
        public int Id { get; set; }
        public string? Sigla { get; set; }
        public required string Tipo { get; set; }
        public required string Nome { get; set; }
    }
}
