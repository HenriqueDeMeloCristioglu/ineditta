namespace Ineditta.API.ViewModels.MapasSindicais.Requests.Comparativos
{
    public class DatabaseRequest
    {
        public int? LocalidadeMunicipioId { get; set; }
        public string? LocalidadeUf { get; set; }
        public int? AtividadeEconomicaId { get; set; }
        public int? NomeDocumentoId { get; set; }
    }
}
