using CSharpFunctionalExtensions;

using System.Globalization;
using System.Text.RegularExpressions;

namespace Ineditta.BuildingBlocks.Core.Domain.ValueObjects
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class CEP : ValueObject
#pragma warning restore S101 // Types should be named in PascalCase
    {
        private CEP(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static Result<CEP> Criar(string input)
        {
            string pattern = @"[^\d]";
            string cepFormated = Regex.Replace(input, pattern, "");

            if (cepFormated.Length != 8)
            {
                return Result.Failure<CEP>("CEP inválido.");
            }

            return Result.Success(new CEP(input));
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
