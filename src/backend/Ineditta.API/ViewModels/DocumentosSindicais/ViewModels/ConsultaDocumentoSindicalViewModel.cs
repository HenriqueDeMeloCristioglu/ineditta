using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class ConsultaDocumentoSindicalViewModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Arquivo { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<string>? SiglasSindicatosLaborais { get; set; }
        public string? SiglasSindicatosLaboraisString {  get; set; }
        public string? SiglasSindicatosPatronaisString { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<int>? SindicatosLaboraisIds { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<int>? SindicatosPatronaisIds { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<string>? SiglasSindicatosPatronais { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<string>? Assuntos { get; set; }
        public string? AssuntosStringForSearch { get; set; }

        [NotSearchableDataTable]
        public IEnumerable<object>? AtividadesEconomicas { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<object>? Abrangencia { get; set; }

        public DateTime DataUpload { get; set; }
        public DateOnly DataValidadeInicial { get; set; }
        public DateOnly DataValidadeFinal { get; set; }
        public string? Descricao { get; set; }
        public string? Comentarios { get; set; }
        public DateOnly? DataInclusao { get; set; }
        public string? MunicipiosEstabelecimentos { get; set; }
    }
}