namespace Ineditta.API.ViewModels.SindicatosLaborais.ViewModels
{
    public class SindicatoLaboralBaseExistenteViewModel
    {
        public int Id { get; set; }
        public required string Sigla { get; set; }
        public string? Denominacao { get; set; }
        public required string Cnpj { get; set; }
    }
}
