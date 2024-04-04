using System.Globalization;
using System.Text.RegularExpressions;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.Sindicatos.Base.ValueObjects
{
    public class CodigoSindical : ValueObject
    {
        private const string Formatador = "{0:000\\.000\\.000\\.00000\\-0}";

        private CodigoSindical(string valor)
        {
            Valor = valor;
        }

        public string Valor { get; private set; }

        public static Result<CodigoSindical> Criar(string valor)
        {
            string codigoLimpo = Limpar(valor);

            if (codigoLimpo.Length < 15)
            {
                return Result.Failure<CodigoSindical>("Código sindical inválido");
            }

            var codigo = new CodigoSindical(codigoLimpo);

            return Result.Success(codigo);
        }

        private static string Limpar(string codigo)
        {
            return Regex.Replace(codigo, @"\s", "");
        }

        public static string Formatar(string valor)
        {
            if (string.IsNullOrEmpty(valor) || !ulong.TryParse(valor, out var codigo))
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.CurrentCulture, Formatador, codigo);
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Valor;
        }
    }
}
