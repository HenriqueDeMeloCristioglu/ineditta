using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DocumentoSindicalDataTableProcessadosViewModel
    {
        public int Id { get; set; }
        public required string NomeDocumento { get; set; }
        public IEnumerable<SindicatoLaboral>? NomeSindicatoLaboral { get; set; }
        public IEnumerable<SindicatoPatronal>? NomeSindicatoPatronal { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public DateOnly? DataAssinatura { get; set; }
        public DateOnly? DataSla { get; set; }
        public string? NomeUsuarioAprovador { get; set; }
        public IEnumerable<Cnae>? CnaeDocs { get; set; }
        public IEnumerable<int>? SubclasseCodigo { get; set; }
    }
}
