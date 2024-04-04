namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DocumentoSindicalProcessadoViewModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; } = null!;
        public DateTime? DataUpload { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public DateTime? DataAprovacaoClausula { get; set; }
        public IEnumerable<string>? SindicatosPatronaisSiglas { get; set; }
        public IEnumerable<string>? SindicatosLaboraisSiglas { get; set; }
        public IEnumerable<string>? SindicatosLaboraisMunicipios { get; set; }
        public string? Arquivo { get; set; }
    }
}
