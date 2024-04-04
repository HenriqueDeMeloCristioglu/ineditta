using CSharpFunctionalExtensions;

namespace Ineditta.Application.TiposEtiquetas.Entities
{
    public class TipoEtiqueta : Entity
    {
        private TipoEtiqueta(string nome)
        {
            Nome = nome;
        }

        public string Nome { get; set; }

        internal static Result<TipoEtiqueta> Criar(string nome)
        {
            if (nome is null) return Result.Failure<TipoEtiqueta>("Nome não pode ser nula");

            var tipoEtiqueta = new TipoEtiqueta(nome);

            return Result.Success(tipoEtiqueta);
        }

        internal Result Atualizar(string nome)
        {
            if (nome is null) return Result.Failure("Nome não pode ser nula");

            Nome = nome;

            return Result.Success();
        }
    }
}
