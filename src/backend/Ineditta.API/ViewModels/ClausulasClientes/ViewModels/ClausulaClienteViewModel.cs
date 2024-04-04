namespace Ineditta.API.ViewModels.ClausulasClientes.ViewModels
{
    public class ClausulaClienteViewModel
    {
        public string NomeUsuario { get; set; } = null!;
        public string Texto { get; set; } = null!;
        public DateOnly DataInclusao { get; set; }
    }
}
