namespace Ineditta.API.ViewModels.AcompanhamentosCct.ViewModels.NegociacoesFases
{
    public class NegociacoesPorUfViewModel
    {
        public string Uf { get; set; } = null!;
        public List<long> Id { get; set; } = null!;
        public int Quantidade { get; set; }
    }
}
