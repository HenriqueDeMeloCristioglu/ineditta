using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.AIs.Clausulas.UseCases.Delete
{
    public class DeleteIAClausulaRequestHandler : BaseCommandHandler, IRequestHandler<DeleteIAClausulaRequest, Result>
    {
        private readonly IIAClausulaRepository _iAClausulaRepository;

        public DeleteIAClausulaRequestHandler(IUnitOfWork unitOfWork, IIAClausulaRepository iAClausulaRepository) : base(unitOfWork)
        {
            _iAClausulaRepository = iAClausulaRepository;
        }

        public async Task<Result> Handle(DeleteIAClausulaRequest request, CancellationToken cancellationToken)
        {
            var iaClausula = await _iAClausulaRepository.ObterPorIdAsync(request.Id);
            if (iaClausula is null)
            {
                return Result.Failure("O ia clasula request de id " + request.Id + " não existe");
            }

            await _iAClausulaRepository.DeleteAsync(iaClausula);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
