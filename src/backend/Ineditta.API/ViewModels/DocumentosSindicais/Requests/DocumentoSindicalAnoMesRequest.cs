namespace Ineditta.API.ViewModels.DocumentosSindicais.Requests
{
    public class DocumentoSindicalAnoMesRequest
    {
        public bool? PorUsuario { get; set; }
        public IEnumerable<int>? GruposEconomicosIds { get; set; }
        public IEnumerable<int>? EmpresasIds { get; set; }
        public IEnumerable<int>? EstabelecimentosIds { get; set; }
        public IEnumerable<int>? AtividadesEconomicasIds { get; set; }
        public IEnumerable<int>? LocalizacoesMunicipiosIds { get; set; }
        public IEnumerable<string>? LocalizacoesEstadosUfs { get; set; }
        public IEnumerable<int>? NomeDocumentoIds { get; set; }
        public IEnumerable<int>? SindLaboralIds { get; set; }
        public IEnumerable<int>? SindPatronalIds { get; set; }
        public bool? Processados { get; set; }
    }
}
