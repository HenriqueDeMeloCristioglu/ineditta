using Ineditta.Application.Documentos.Sindicais.Dtos;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DocumentoSindicalViewModel
    {
        public int Id { get; set; }
        public DateOnly DataInicial { get; set; }
        public DateOnly? DataLiberacaoClausulas { get; set; }
        public DateOnly? DataFinal { get; set; }
        public IEnumerable<Abrangencia>? Abrangencias { get; set; }
        public IEnumerable<Estabelecimento>? Estabelecimentos { get; set; }
    }
}
