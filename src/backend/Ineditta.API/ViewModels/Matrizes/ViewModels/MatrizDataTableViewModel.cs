namespace Ineditta.API.ViewModels.Matrizes.ViewModels
{
    public class MatrizDataTableViewModel
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Cnpj { get; set; }
        public string? Uf { get; set; }
        public string? GrupoEconomico { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime? DataInativacao { get; set; }
    }
}
