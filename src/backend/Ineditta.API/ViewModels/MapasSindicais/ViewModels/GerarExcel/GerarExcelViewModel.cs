using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;
using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.API.ViewModels.MapasSindicais.ViewModels.GerarExcel
{
    public class GerarExcelViewModel
    {
        public int? Id { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<SindLaboral>? SindLaboral { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<SindPatronal>? SindPatronal { get; set; }
        public string? DataBase { get; set; }
        public DateOnly? ValidadeFinal { get; set; }
        public DateOnly? DataLiberacao { get; set; }
        public DateOnly? Aprovacao { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<CnaeDoc>? CnaeDoc { get; set; }
        public string? UfDoc { get; set; }
        public string? TituloDoc { get; set; }
        [NotSearchableDataTable]
        public IEnumerable<Abrangencia>? Abrangencias { get; set; }
    }
}