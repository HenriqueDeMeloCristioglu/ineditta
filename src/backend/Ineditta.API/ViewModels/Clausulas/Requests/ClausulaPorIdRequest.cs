namespace Ineditta.API.ViewModels.Clausulas.Requests
{
    public class ClausulaPorIdRequest
    {
        public IEnumerable<int> ClausulaIds { get; set; } = new List<int>();
        public string? PalavraChave { get; set; }
    }
}
