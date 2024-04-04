using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.DocumentosSindicais.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.AIs.DocumentosSindicais.Services.ClassificacaoClausula
{
    public interface IClassificacaoClausulaService
    {
        ValueTask<Result<IEnumerable<ClassificacaoClausulaDto>, Error>> ClassificarClausulas(IEnumerable<ClausulaDto> clausulas, CancellationToken cancellationToken = default);
        ValueTask<Result<ClassificacaoClausulaDto, Error>> ClassificarClausula(ClausulaDto clausula, CancellationToken cancellationToken = default);
    }
}
