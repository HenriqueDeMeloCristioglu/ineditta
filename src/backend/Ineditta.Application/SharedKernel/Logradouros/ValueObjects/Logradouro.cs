using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.SharedKernel.Logradouros.ValueObjects
{
    public class Logradouro : ValueObject<Logradouro>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Logradouro()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        private Logradouro(string? endereco, string? regiao, string? bairro, CEP cep)
        {
            Endereco = endereco;
            Regiao = regiao;
            Bairro = bairro;
            Cep = cep;
        }

        public string? Endereco { get; private set; }
        public string? Regiao { get; private set; }
        public string? Bairro { get; private set; }
        public CEP Cep { get; private set; }

        public static Result<Logradouro> Criar(string? endereco, string regiao, string bairro, CEP cep)
        {
            var logradouro = new Logradouro(endereco, regiao, bairro, cep);

            return Result.Success(logradouro);
        }

        protected override bool EqualsCore(Logradouro other)
        {
            throw new NotImplementedException();
        }

        protected override int GetHashCodeCore()
        {
            throw new NotImplementedException();
        }
    }
}
