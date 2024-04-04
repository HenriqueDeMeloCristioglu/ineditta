namespace Ineditta.API.ViewModels.Localicacoes.ViewModels
{
    public class LocalizacaoViewModel
    {
        public int Id { get; set; }
        public int? IdLocalizacao { get; set; }
        public string? CodigoPais { get; set; }
        public string? Pais { get; set; }
        public string? CodigoRegiao { get; set; }
        public string? Regiao { get; set; }
        public string? CodigoUf { get; set; }
        public string? Estado { get; set; }
        public string? Uf { get; set; }
        public string? CodigoMunicipio { get; set; }
        public string? Municipio { get; set; }
    }
}
