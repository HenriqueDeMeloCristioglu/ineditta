using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Idempotency.Database;
using Ineditta.BuildingBlocks.Core.Idempotency.Web;

using MediatR;

namespace Ineditta.BuildingBlocks.Core.Idempotency.Pipelines
{
#pragma warning disable S2326 // Unused type parameters should be removed
    public class IdempotentPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, IResult<Unit, Error>>
#pragma warning restore S2326 // Unused type parameters should be removed
        where TRequest: IdempotentRequest
    {
        private readonly IIdempotentRepository _idempotentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public IdempotentPipelineBehavior(IIdempotentRepository idempotentRepository, IUnitOfWork unitOfWork)
        {
            _idempotentRepository = idempotentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<Unit, Error>> Handle(TRequest request, RequestHandlerDelegate<IResult<Unit, Error>> next, CancellationToken cancellationToken)
        {
            if (await _idempotentRepository.AnyAsync(request.IdempotentToken, cancellationToken))
            {
                return Result.Failure<Unit, Error>(Errors.Http.Duplicated());
            }

            await _idempotentRepository.AddAsync(new IdempotentModel(request.IdempotentToken, request.GetType().Name), cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            return await next();
        }
    }
}
