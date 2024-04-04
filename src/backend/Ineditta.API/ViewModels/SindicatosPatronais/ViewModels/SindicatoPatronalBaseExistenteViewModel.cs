namespace Ineditta.API.ViewModels.SindicatosPatronais.ViewModels
{
    public class SindicatoPatronalBaseExistenteViewModel
    {
        public int Id { get; set; }
        public required string Sigla { get; set; }
        public string? Denominacao { get; set; }
        public string? Cnpj { get; set; }
    }
}
