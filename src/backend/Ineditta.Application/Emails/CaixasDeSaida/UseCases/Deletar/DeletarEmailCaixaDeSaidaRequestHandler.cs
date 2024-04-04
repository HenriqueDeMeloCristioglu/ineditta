using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Emails.CaixasDeSaida.UseCases.Deletar
{
    public class DeletarEmailCaixaDeSaidaRequestHandler : BaseCommandHandler, IRequestHandler<DeletarEmailCaixaDeSaidaRequest, Result>
    {
        private readonly IEmailCaixaDeSaidaRepository _emailCaixaDeSaidaRepository;
        public DeletarEmailCaixaDeSaidaRequestHandler(IUnitOfWork unitOfWork, IEmailCaixaDeSaidaRepository emailCaixaDeSaidaRepository) : base(unitOfWork)
        {
            _emailCaixaDeSaidaRepository = emailCaixaDeSaidaRepository;
        }

        public async Task<Result> Handle(DeletarEmailCaixaDeSaidaRequest request, CancellationToken cancellationToken)
        {
            var result = await _emailCaixaDeSaidaRepository.DeletarAsync(request.EmailCaixaDeSaida);

            await CommitAsync(cancellationToken);

            if (result.IsFailure)
            {
                return result;
            }

            return Result.Success();
        }
    }
}
