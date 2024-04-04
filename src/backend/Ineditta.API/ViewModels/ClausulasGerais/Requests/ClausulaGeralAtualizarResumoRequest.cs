namespace Ineditta.API.ViewModels.ClausulasGerais.Requests
{
    public class ClausulaGeralAtualizarResumoRequest
    {
        public int EstruturaId { get; set; }
        public int DocumentoId { get; set; }
        public string Texto { get; set; } = null!;
    }
}
