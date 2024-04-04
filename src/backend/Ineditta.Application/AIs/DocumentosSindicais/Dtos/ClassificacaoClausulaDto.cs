namespace Ineditta.Application.AIs.DocumentosSindicais.Dtos
{
    public class ClassificacaoClausulaDto
    {
        public ClassificacaoClausulaDto(int classificacao, string retornoIA, ClausulaDto clausula)
        {
            Classificacao = classificacao;
            RetornoIA = retornoIA;
            Clausula = clausula;
        }

        public int Classificacao { get; private set; }
        public string RetornoIA { get; private set; }
        public ClausulaDto Clausula { get; private set; }
    }
}
