using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.IncluirEstabelecimentos
{
    public class IncluirEstabelecimentosRequest : IRequest<Result<Unit, Error>>
    {
    }
}
