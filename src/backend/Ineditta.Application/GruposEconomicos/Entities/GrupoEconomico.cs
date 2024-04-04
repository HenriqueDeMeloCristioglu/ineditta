using CSharpFunctionalExtensions;

namespace Ineditta.Application.GruposEconomicos.Entities
{
    public class GrupoEconomico : Entity<int>
    {
        private GrupoEconomico(string nome, string? logotipo)
        {
            Nome = nome;
            Logotipo = logotipo;
        }

        protected GrupoEconomico()
        {

        }

        public string Nome { get; private set; } = null!;
        public string? Logotipo { get; private set; }

        public static Result<GrupoEconomico> Criar(string nome, string? logotipo)
        {
            if (nome is null) return Result.Failure<GrupoEconomico>("Informe o Nome do Grupo");

            var grupoEconomico = new GrupoEconomico(nome, logotipo);

            return Result.Success(grupoEconomico);
        }

        public Result Atualizar(string nome, string? logotipo)
        {
            if (nome is null) return Result.Failure<GrupoEconomico>("Informe o Nome do Grupo");

            Nome = nome;
            Logotipo = logotipo;

            return Result.Success();
        }
    }
}
