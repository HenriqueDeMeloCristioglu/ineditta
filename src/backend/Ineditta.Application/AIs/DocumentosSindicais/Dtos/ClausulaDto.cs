namespace Ineditta.Application.AIs.DocumentosSindicais.Dtos
{
    public class ClausulaDto
    {
        public ClausulaDto(string? grupo, string? subGrupo, int numero, string nome, string descricao)
        {
            Grupo = grupo;
            SubGrupo = subGrupo;
            Numero = numero;
            Nome = nome;
            Descricao = descricao;
        }

        public string? Grupo { get; private set; }
        public string? SubGrupo { get; private set; }
        public int Numero { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
    }
}
