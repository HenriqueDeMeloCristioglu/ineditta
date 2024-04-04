namespace Ineditta.API.ViewModels.Cnaes.ViewModels
{
    public class CnaePorIdViewModel
    {
        public int Id { get; set; }
        public int Divisao { get; set; }
        public string? DescricaoDivisao { get; set; }
        public int Subclasse { get; set; }
        public string? DescricaoSubclasse { get; set; }
        public string? Categoria { get; set; }
    }
}
