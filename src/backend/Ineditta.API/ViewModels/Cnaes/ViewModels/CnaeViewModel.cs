using Ineditta.BuildingBlocks.Core.Web.API.Datatables.Attributes;

namespace Ineditta.API.ViewModels.Cnaes.ViewModels
{
    public class CnaeViewModel
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; set; }
        public int Divisao { get; set; }
        public string Descricao { get; set; }
        public int Subclasse { get; set; }
        public string DescricaoSubClasse { get; set; }
        public string? Categoria { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }

        [NotSearchableDataTable]
        public List<string> SiglasSindicatosPatronaisFiltro { get; set; }
        [NotSearchableDataTable]
        public List<string> SiglasSindicatosLaboraisFiltro { get; set; }

        [NotSearchableDataTable]
        public string? SiglasSindicatosPatronaisFiltrados => SiglasSindicatosPatronaisFiltro != null && SiglasSindicatosPatronaisFiltro.Any() && !string.IsNullOrEmpty(SiglasSindicatosPatronais) ? string.Join(", ", SiglasSindicatosPatronais.Split(", ").Where(SiglasSindicatosPatronaisFiltro.Contains)) : SiglasSindicatosPatronais;
        [NotSearchableDataTable]
        public string? SiglasSindicatosLaboraisFiltrados => SiglasSindicatosLaboraisFiltro != null && SiglasSindicatosLaboraisFiltro.Any() && !string.IsNullOrEmpty(SiglasSindicatosLaborais) ? string.Join(", ", SiglasSindicatosLaborais.Split(", ").Where(SiglasSindicatosLaboraisFiltro.Contains)) : SiglasSindicatosLaborais;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
