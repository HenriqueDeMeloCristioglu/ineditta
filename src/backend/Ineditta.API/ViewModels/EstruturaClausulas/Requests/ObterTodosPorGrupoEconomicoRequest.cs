namespace Ineditta.API.ViewModels.EstruturaClausulas.Requests
{
    public class ObterTodosPorGrupoEconomicoRequest
    {
        public IEnumerable<int>? GrupoClausulaId { get; set; }
        public bool Calendario { get; set; }
        public bool ClausulaResumivel { get; set; }
        public bool ClausulaNaoConsta { get; set; }
    }
}
