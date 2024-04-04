using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.Documentos.AtividadesEconomicas.UseCases.IncluirCnaesDocmentos
{
    public class IncluirCnaesDocumentosRequest : IRequest<Result<Unit, Error>>
    {
    }
}
