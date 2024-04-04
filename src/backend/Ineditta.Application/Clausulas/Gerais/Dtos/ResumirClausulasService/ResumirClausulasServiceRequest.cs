namespace Ineditta.Application.Clausulas.Gerais.Dtos.ResumirClausulas
{
    public class ResumirClausulasServiceRequest
    {
        public IEnumerable<string> Textos { get; set; } = null!;
        public string InstrucoesIA { get; set; } = null!;
        public int MaximoPalavrasIA { get; set; }
    }
}
