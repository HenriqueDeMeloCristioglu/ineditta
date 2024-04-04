namespace Ineditta.API.ViewModels.ClientesUnidades.Requests
{
    public class FiltrarEstabelecimentosSelecionadosRequest
    {
        public IEnumerable<int>? GruposIds { get; set; }
        public IEnumerable<int>? EstabelecimentoIds { get; set; }
    }
}
