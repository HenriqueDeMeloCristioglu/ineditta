using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.BuildingBlocks.Core.Idempotency.Web
{
    public abstract class IdempotentRequest : IRequest<IResult<Unit, Error>>
    {
        public Guid IdempotentToken { get; set; }
    }
}
