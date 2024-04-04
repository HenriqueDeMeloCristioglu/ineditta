using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DocumentoSindicalNegociacaoEncerradasViewModel
    {
        public int Id { get; set; }
        public string? SiglaDoc { get; set; }
        public IEnumerable<string>? SindicatosPatronaisSiglas { get; set; }
        public IEnumerable<string>? SindicatosLaboraisSiglas { get; set; }
        public IEnumerable<string>? SindicatosLaboraisMunicipios { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public string? DataBase { get; set; } = null!;
        [NotSearchableDataTable]
        public string? Arquivo { get; set; }
    }
}
