namespace Ineditta.Application.AIs.DocumentosSindicais.Dtos
{
    public class ClassificacaoClausulaResponseDto
    {
        public ClassificacaoClausulaResponseDto(string grupo, int id, string estrutura)
        {
            Grupo = grupo;
            Id = id;
            Estrutura = estrutura;
        }

        public string Grupo { get; private set; }
        public int Id { get; private set; }
        public string Estrutura { get; private set; }
    }
}
