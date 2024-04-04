namespace Ineditta.API.ViewModels.Clausulas.Requests
{
    public class ClausulaDatasBasesRequest
    {
        public IEnumerable<int>? SindLaboralIds { get; set; }
        public IEnumerable<int>? SindPatronalIds { get; set; }
        public IEnumerable<int>? GrupoEconomicoIds { get; set; }
    }
}
