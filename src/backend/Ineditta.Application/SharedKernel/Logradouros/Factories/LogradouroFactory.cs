using CSharpFunctionalExtensions;

using Ineditta.Application.SharedKernel.Logradouros.ValueObjects;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.SharedKernel.Logradouros.Factories
{
    public class LogradouroFactory
    {
        protected LogradouroFactory()
        {

        }

        public static Result<Logradouro> Criar (string? endereco, string regiao, string bairro, string cep)
        {
            var resultCep = CEP.Criar(cep);

            if (resultCep.IsFailure)
            {
                return Result.Failure<Logradouro>("CEP inválido");
            }

            var result = Logradouro.Criar(endereco, regiao, bairro, resultCep.Value);

            if (result.IsFailure)
            {
                return Result.Failure<Logradouro>("Logradouro inválido");
            }

            return Result.Success(result.Value);
        }
    }
}
