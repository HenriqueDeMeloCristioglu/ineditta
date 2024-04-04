namespace Ineditta.Application.AIs.DocumentosSindicais.Dtos
{
    public class QuebraClausulaDto
    {
        public QuebraClausulaDto(int quantidadeClausulas, IEnumerable<ClausulaDto> clausulas, bool utilizouIA)
        {
            QuantidadeClausulas = quantidadeClausulas;
            Clausulas = clausulas;
            UtilizouIA = utilizouIA;
        }

        public int QuantidadeClausulas { get; private set; }
        public IEnumerable<ClausulaDto> Clausulas { get; private set; }
        public bool UtilizouIA { get; private set; }
    }
}
