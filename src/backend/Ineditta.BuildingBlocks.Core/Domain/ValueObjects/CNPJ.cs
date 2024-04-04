using System.Globalization;

using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Extensions;

namespace Ineditta.BuildingBlocks.Core.Domain.ValueObjects
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class CNPJ : ValueObject
#pragma warning restore S101 // Types should be named in PascalCase
    {
        private const string Formatador = "{0:00\\.000\\.000\\/0000\\-00}";

        private CNPJ(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static Result<CNPJ> Criar(string valor)
        {
            var result = new CNPJ(valor.OnlyNumbers());

            return Result.Success(result);
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Value;
        }

        public static string Formatar(string valor)
        {
            if (string.IsNullOrEmpty(valor) || !ulong.TryParse(valor, out var cnpj))
            {
                return string.Empty;
            }

            return string.Format(CultureInfo.CurrentCulture, Formatador, cnpj);
        }
    }
}
