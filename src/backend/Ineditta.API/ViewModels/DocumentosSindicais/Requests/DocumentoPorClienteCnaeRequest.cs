namespace Ineditta.API.ViewModels.DocumentosSindicais.Requests
{
    public class DocumentoPorClienteCnaeRequest
    {
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? EmpresasIds { get; set; }
        public IEnumerable<int>? AtividadesEconomicasIds { get; set; }
    }
}
