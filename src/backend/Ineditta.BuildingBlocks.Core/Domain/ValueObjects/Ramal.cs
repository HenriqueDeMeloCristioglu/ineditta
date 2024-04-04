using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Extensions;

namespace Ineditta.BuildingBlocks.Core.Domain.ValueObjects
{
    public class Ramal : ValueObject
    {
        public Ramal(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; set; }

        public static Result<Ramal> Criar(string numero)
        {
            if (numero.Length < 4)
            {
                return Result.Failure<Ramal>("Ramal deve ter no mínimo 4 caracteres");
            }

            if (numero.Length > 6)
            {
                return Result.Failure<Ramal>("Ramal deve ter no máximo 5 caracteres");
            }

            var ramal = new Ramal(numero);

            return Result.Success(ramal);
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Valor;
        }
    }
}
