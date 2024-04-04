using CSharpFunctionalExtensions;

namespace Ineditta.Application.Etiquetas.Entities
{
    public class Etiqueta : Entity
    {
        private Etiqueta(string nome, long tipoEtiquetaId)
        {
            Nome = nome;
            TipoEtiquetaId = tipoEtiquetaId;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Etiqueta()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public string Nome { get; private set; }
        public long TipoEtiquetaId { get; private set; }

        internal static Result<Etiqueta> Criar(string nome, long tipoEtiquetaId)
        {
            if (nome is null) return Result.Failure<Etiqueta>("Nome não pode ser nulo");
            if (tipoEtiquetaId <= 0) return Result.Failure<Etiqueta>("o Tipo de Etiqueta não pode ser nulo");

            var etiqueta = new Etiqueta(nome, tipoEtiquetaId);

            return Result.Success(etiqueta);
        }

        internal Result Atualizar(string nome, long tipoEtiquetaId)
        {
            if (nome is null) return Result.Failure<Etiqueta>("Nome não pode ser nulo");
            if (tipoEtiquetaId <= 0) return Result.Failure<Etiqueta>("o Tipo de Etiqueta não pode ser nulo");

            Nome = nome;
            TipoEtiquetaId = tipoEtiquetaId;

            return Result.Success();
        }
    }
}
