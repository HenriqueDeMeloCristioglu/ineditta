namespace Ineditta.API.ViewModels.Cnaes.ViewModels
{
    public class CnaesPorSindicatosViewModel
    {
        public int Id { get; set; }
        public required string SiglaSindicato { get; set; }
        public int Divisao { get; set; }
        public required string Descricao { get; set; }
        public int Subclasse { get; set; }
        public required string DescricaoSubClasse { get; set; }
        public required string Categoria { get; set; }
    }
}
