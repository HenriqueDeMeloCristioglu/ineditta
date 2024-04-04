namespace Ineditta.API.ViewModels.BasesTerritoriais.SindicatosPatronais
{
    public class BaseTerritorialSindicatoPatronalViewModel
    {
        public int Id { get; set; }
        public string? Sigla { get; set; }
        public DateOnly DataInicial { get; set; }
        public DateOnly? DataFinal { get; set; }
        public string? Municipio { get; set; }
        public string? DescricaoSubClasse { get; set; }
        public int LocalizacaoId { get; set; }
        public string? Pais { get; set; }
        public string? Regiao { get; set; }
        public string? Estado { get; set; }
        public string? CodigoUf { get; set; }
        public int? CnaeId { get; set; }
        public string? SubclasseCnae { get; set; }
        public string? DescricaoCnae { get; set; }
    }
}
