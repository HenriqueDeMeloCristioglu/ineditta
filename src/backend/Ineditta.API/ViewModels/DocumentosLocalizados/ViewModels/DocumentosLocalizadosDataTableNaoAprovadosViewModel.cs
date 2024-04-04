namespace Ineditta.API.ViewModels.DocumentosLocalizados.ViewModels
{
    public class DocumentosLocalizadosDataTableNaoAprovadosViewModel
    {
        public long Id { get; set; }
        public required string Nome { get; set; }
        public required string Caminho { get; set; }
        public string? Origem { get; set; }
        public DateTime DataRegistro { get; set; }
    }
}
