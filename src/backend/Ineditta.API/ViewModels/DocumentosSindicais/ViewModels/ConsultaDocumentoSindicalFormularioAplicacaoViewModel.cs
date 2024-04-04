using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class ConsultaDocumentoSindicalFormularioAplicacaoViewModel
    {
        public int Id { get; set; }
        [NotSearchableDataTable]
        public List<object>? AtividadesEconomicas { get; set; }
        public string? Nome { get; set; }
        public string? Arquivo { get; set; }
        [NotSearchableDataTable]
        public List<object>? SindicatosPatronais { get; set; }
        [NotSearchableDataTable]
        public List<object>? SindicatosLaborais { get; set; }
        public DateOnly? DataLiberacao { get; set; }
        public DateOnly DataValidadeInicial { get; set; }
        public DateOnly DataValidadeFinal { get; set; }
        public string? Comentario { get; set; }
        public string? Tipo { get; set; }
    }
}