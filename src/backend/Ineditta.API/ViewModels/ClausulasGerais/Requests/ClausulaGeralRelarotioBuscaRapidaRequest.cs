namespace Ineditta.API.ViewModels.ClausulasGerais.Requests
{
    public class ClausulaGeralRelarotioBuscaRapidaRequest
    {
        public IEnumerable<int> ClausulasIds { get; set; } = null!;
        public DateOnly? DataProcessamentoInicial { get; set; }
        public DateOnly? DataProcessamentoFinal { get; set; }
    }
}
