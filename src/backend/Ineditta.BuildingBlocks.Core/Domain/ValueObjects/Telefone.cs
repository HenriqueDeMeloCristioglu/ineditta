using System.Globalization;
using System.Text.RegularExpressions;

using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Domain.ValueObjects
{
    public class Telefone : ValueObject
    {
        private const string FormatadorMovel = "{0:(00)\\ 00000\\-0000}";
        private const string FormatadorFixo = "{0:(00)\\ 0000\\-0000}";

        protected Telefone() { }
        private Telefone(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; private set; } = null!;

        public static Result<Telefone> Criar(string valor)
        {
            var valorLimpo = Limpar(valor);

            if (valorLimpo.Length < 10)
            {
                return Result.Failure<Telefone>("Telefone deve ter no mínimo 10 caracteres");
            }

            if (valorLimpo.Length > 11)
            {
                return Result.Failure<Telefone>("Telefone deve ter no máximo 11 caracteres");
            }

            if (!long.TryParse(valorLimpo, out long numero))
            {
                return Result.Failure<Telefone>("Telefone não é um número");
            } else
            {
                var telefone = new Telefone(valorLimpo);

                return Result.Success(telefone);
            }
        }

        private static string Limpar(string numero)
        {
            return Regex.Replace(numero, @"\s", "", RegexOptions.None, matchTimeout: TimeSpan.FromSeconds(20));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Valor;
        }

        public static string Formatar(string valor)
        {
            if (string.IsNullOrEmpty(valor) || !ulong.TryParse(valor, out var tel))
            {
                return string.Empty;
            }

            if (valor.Length == 11)
            {
                return string.Format(CultureInfo.CurrentCulture, FormatadorMovel, tel);
            }

            if (valor.Length == 10)
            {
                return string.Format(CultureInfo.CurrentCulture, FormatadorFixo, tel);
            }

            return valor;
        }
    }
}
