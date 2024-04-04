namespace Ineditta.API.ViewModels.ClausulasClientes.ViewModels
{
    public class ClausulaClientePorIdViewModel
    {
        public long Id { get; set; }
        public string Texto { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public DateTime DataInclusao { get; set; }
    }
}
