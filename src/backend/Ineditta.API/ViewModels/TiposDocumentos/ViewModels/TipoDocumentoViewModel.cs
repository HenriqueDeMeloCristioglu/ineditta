namespace Ineditta.API.ViewModels.TiposDocumentos.ViewModels
{
    public class TipoDocumentoViewModel
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public bool Processado { get; set; }
    }
}
