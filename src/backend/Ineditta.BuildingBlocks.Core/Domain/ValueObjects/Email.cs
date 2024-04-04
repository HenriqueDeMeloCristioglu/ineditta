using System.Text.RegularExpressions;

using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Domain.ValueObjects
{
    public class Email: ValueObject<Email>
    {
        private const string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private Email(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; private set; }

        protected override bool EqualsCore(Email other)
        {
            return other.Valor == Valor;
        }

        public static Result<Email> Criar(string email)
        {
            if (!Validar(email))
            {
                return Result.Failure<Email>("Email inválido");
            }
            //validar se é email válido

            var result = new Email(email);

            return Result.Success(result);
        }

        protected override int GetHashCodeCore()
        {
            return Valor.GetHashCode();
        }

        private static bool Validar(string email)
        {
            return Regex.IsMatch(email, pattern);
        }

        public static implicit operator string(Email email)
        {
            return email.Valor;
        }
    }
}
