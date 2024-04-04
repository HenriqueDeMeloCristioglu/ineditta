using CSharpFunctionalExtensions;

namespace Ineditta.Application.Cnaes.Entities
{
    public class Cnae : Entity<int>
    {
        private Cnae(int divisao, string descricaoDivisao, int subClasse, string descricaoSubClasse, string categoria)
        {
            Divisao = divisao;
            DescricaoDivisao = descricaoDivisao;
            SubClasse = subClasse;
            DescricaoSubClasse = descricaoSubClasse;
            Categoria = categoria;
        }
        public int Divisao { get;  private set; }
        public string DescricaoDivisao { get; private set; }
        public int SubClasse { get; private set; }
        public string DescricaoSubClasse { get; private set; }
        public string Categoria { get; private set; }

        public static Result<Cnae> Criar(int divisao, string descricaoDivisao, int subClasse, string descricaoSubClasse, string categoria)
        {
            if (divisao <= 0)
            {
                return Result.Failure<Cnae>("Informe a divisão");
            }

            if (descricaoDivisao.Length > 500)
            {
                return Result.Failure<Cnae>("Descrição da divisão deve ter no máximo 500 caracteres");
            }

            if (subClasse <= 0)
            {
                return Result.Failure<Cnae>("Informe a Sub Classe");
            }

            if (descricaoSubClasse.Length > 500)
            {
                return Result.Failure<Cnae>("Descrição da sub classe deve ter no máximo 500 caracteres");
            }

            if (string.IsNullOrEmpty(categoria))
            {
                return Result.Failure<Cnae>("Informe uma categoria");
            }

            if (categoria.Length > 200)
            {
                return Result.Failure<Cnae>("Categoria deve ter no máximo 200 caracteres");
            }

            var cnae = new Cnae(divisao, descricaoDivisao, subClasse, descricaoSubClasse, categoria);

            return Result.Success(cnae);
        }

        public Result Atualizar(int divisao, string descricaoDivisao, int subClasse, string descricaoSubClasse, string categoria)
        {
            if (divisao <= 0)
            {
                return Result.Failure("Informe a divisão");
            }

            if (descricaoDivisao.Length > 500)
            {
                return Result.Failure("Descrição da divisão deve ter no máximo 500 caracteres");
            }

            if (subClasse <= 0)
            {
                return Result.Failure("Informe a Sub Classe");
            }

            if (descricaoSubClasse.Length > 500)
            {
                return Result.Failure("Descrição da sub classe deve ter no máximo 500 caracteres");
            }

            if (string.IsNullOrEmpty(categoria))
            {
                return Result.Failure("Informe uma categoria");
            }

            if (categoria.Length > 200)
            {
                return Result.Failure("Categoria deve ter no máximo 200 caracteres");
            }

            Divisao = divisao;
            DescricaoDivisao = descricaoDivisao;
            SubClasse = subClasse;
            DescricaoSubClasse = descricaoSubClasse;
            Categoria = categoria;

            return Result.Success();
        }
    }
}
