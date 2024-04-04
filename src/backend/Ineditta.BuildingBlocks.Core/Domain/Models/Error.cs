using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Domain.Models
{
#pragma warning disable CA1716 // Identifiers should not match keywords
    public sealed class Error : ValueObject
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        private const string Separator = "||";

        public string Code { get; }
        public string Message { get; }

        internal Error(string code, string message)
        {
            Code = code;
            Message = message;
        }

        public static Error Create(string code, string message)
        {
            return new Error(code, message);
        }

        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return Code;
        }

        public string Serialize()
        {
            return $"{Code}{Separator}{Message}";
        }

        public static Error Deserialize(string serialized)
        {
            if (serialized == "A non-empty request body is required.")
                return Errors.General.ValueIsRequired();

            string[] data = serialized.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

#pragma warning disable CA2201 // Do not raise reserved exception types
            return data.Length < 2 ? throw new Exception($"Invalid error serialization: '{serialized}'") : new Error(data[0], data[1]);
#pragma warning restore CA2201 // Do not raise reserved exception types
        }
    }
}
