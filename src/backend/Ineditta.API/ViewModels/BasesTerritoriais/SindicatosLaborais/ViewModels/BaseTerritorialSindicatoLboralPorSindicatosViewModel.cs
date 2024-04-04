namespace Ineditta.API.ViewModels.BasesTerritoriais.SindicatosLaborais.ViewModels
{
    public class BaseTerritorialSindicatoLboralPorSindicatosViewModel
    {
        public int Id { get; set; }
        public string Sigla { get; set; } = null!;
        public string Uf { get; set; } = null!;
        public string Municipio { get; set; } = null!;
        public string Pais { get; set; } = null!;
    }
}
