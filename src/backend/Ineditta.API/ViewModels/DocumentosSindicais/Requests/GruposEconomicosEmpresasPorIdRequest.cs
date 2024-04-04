namespace Ineditta.API.ViewModels.DocumentosSindicais.Requests
{
    public class GruposEconomicosEmpresasPorIdRequest
    {
        public string NomeGrupoEconomico { get; set; } = null!;
        public string Empresa { get; set; } = null!;
        public string? Abrangencia { get; set; }
        public string? SindicatosLaborais { get; set; }
        public string? SindicatosPatronais { get; set; }
    }
}
