namespace Ineditta.Repository.ClassesCnaes.Views
{
    public class ClasseCnaeVw
    {
        public int Id { get; set; }
        public int Divisao { get; set; }
        public int Subclasse { get; set; }
        public string DescricaoSubclasse { get; set; }
        public string? Categoria { get; set; }
        public string? SiglasSindicatosPatronais { get; set; }
        public string? SiglasSindicatosLaborais { get; set; }
    }
}
