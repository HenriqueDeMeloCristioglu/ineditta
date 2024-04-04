using Ineditta.API.ViewModels.IAs.Clausulas;

namespace Ineditta.API.ViewModels.IAs.DocumentosSindicais.ViewModels
{
    public class IADocumentoSindicalViewModel
    {
        public long Id { get; set; }
        public int DocumentoReferenciaId { get; set; }
        public DateOnly VigenciaInicial { get; set; }
        public DateOnly? VigenciaFinal { get; set; }
        public string Nome { get; set; } = null!;
        public IEnumerable<IAClausulaViewModel>? Clausulas { get; set; }
    }
}
