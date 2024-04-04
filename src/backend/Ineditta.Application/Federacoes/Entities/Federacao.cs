using CSharpFunctionalExtensions;

namespace Ineditta.Application.Federacoes.Entities
{
    public class Federacao : Entity
    {
        private Federacao(int divisao, string descricaoDivisao, int subClasse, string descricaoSubClasse, string categoria)
        {
            Divisao = divisao;
            DescricaoDivisao = descricaoDivisao;
            SubClasse = subClasse;
            DescricaoSubClasse = descricaoSubClasse;
            Categoria = categoria;
        }
        public int Divisao { get; private set; }
        public string DescricaoDivisao { get; private set; }
        public int SubClasse { get; private set; }
        public string DescricaoSubClasse { get; private set; }
        public string Categoria { get; private set; }

        public static Result<Federacao> Criar(int divisao, string descricaoDivisao, int subClasse, string descricaoSubClasse, string categoria)
        {
            if (divisao <= 0)
            {
                return Result.Failure<Federacao>("Informe a divisão");
            }

            if (descricaoDivisao.Length > 500)
            {
                return Result.Failure<Federacao>("Descrição da divisão deve ter no máximo 500 caracteres");
            }

            if (subClasse <= 0)
            {
                return Result.Failure<Federacao>("Informe a Sub Classe");
            }

            if (descricaoSubClasse.Length > 500)
            {
                return Result.Failure<Federacao>("Descrição da sub classe deve ter no máximo 500 caracteres");
            }

            if (string.IsNullOrEmpty(categoria))
            {
                return Result.Failure<Federacao>("Informe uma categoria");
            }

            if (categoria.Length > 200)
            {
                return Result.Failure<Federacao>("Categoria deve ter no máximo 200 caracteres");
            }

            var federacao = new Federacao(divisao, descricaoDivisao, subClasse, descricaoSubClasse, categoria);

            return Result.Success(federacao);
        }
    }
}
