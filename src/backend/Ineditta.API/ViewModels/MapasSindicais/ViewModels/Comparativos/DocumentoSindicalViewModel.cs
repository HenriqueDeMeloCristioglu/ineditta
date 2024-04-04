using Ineditta.Repository.Clausulas.Geral.Models;

namespace Ineditta.API.ViewModels.MapasSindicais.ViewModels.Comparativos
{
    public class DocumentoSindicalViewModel
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public SindPatronal[]? SindicatosPatronais { get; set; }
        public SindLaboral[]? SindicatosLaborais { get; set; }
        public CnaeDoc[]? AtividadesEconomicas { get; set; }
        public DateTime? DataUpload { get; set; }
        public DateOnly? DataValidadeInicial { get; set; }
        public DateOnly? DataValidadeFinal { get; set; }
        public string? Descricao { get; set; }
        public string? DataBase { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
        public string? CnaesSubclasses { get; set; }
    }
}