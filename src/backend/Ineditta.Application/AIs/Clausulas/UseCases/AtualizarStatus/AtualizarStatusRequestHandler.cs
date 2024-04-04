using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.AIs.Clausulas.UseCases.AtualizarStatus
{
    public class AtualizarStatusRequestHandler : BaseCommandHandler, IRequestHandler<AtualizarStatusRequest, Result>
    {
        private readonly IIAClausulaRepository _iAClausulaRepository;
        public AtualizarStatusRequestHandler(IUnitOfWork unitOfWork, IIAClausulaRepository iAClausulaRepository) : base(unitOfWork)
        {
            _iAClausulaRepository = iAClausulaRepository;
        }

        public async Task<Result> Handle(AtualizarStatusRequest request, CancellationToken cancellationToken)
        {
            var iaClausula = await _iAClausulaRepository.ObterPorIdAsync(request.Id);
            if (iaClausula is null)
            {
                return Result.Failure("Cláusula não encontrada");
            }

            var result = iaClausula.AtualizarStatus(request.Status);
            if (result.IsFailure)
            {
                return result;
            }

            await _iAClausulaRepository.AtualizarAsync(iaClausula);

            await CommitAsync(cancellationToken);

            return Result.Success(result);
        }
    }
}
