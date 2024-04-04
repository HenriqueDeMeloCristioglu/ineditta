using Ineditta.BuildingBlocks.Core.Domain.Models;
using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.DocumentosSindicais.Dtos;

namespace Ineditta.Application.AIs.DocumentosSindicais.Services.QuebraClausula
{
    public interface IQuebraClausulaService
    {
        ValueTask<Result<QuebraClausulaDto, Error>> QuebrarContratoEmClausulas(long documentoSindicalId, CancellationToken cancellationToken = default);
    }
}
