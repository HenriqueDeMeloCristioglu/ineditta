using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Entities;
using Ineditta.Application.Emails.CaixasDeSaida.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

namespace Ineditta.Application.Emails.CaixasDeSaida.UseCases.Incluir
{
    public class IncluirEmailCaixaDeSaidaRequestHandler : BaseCommandHandler, IRequestHandler<IncluirEmailCaixaDeSaidaRequest, Result>
    {
        private readonly IEmailCaixaDeSaidaRepository _emailCaixaDeSaidaRepository;
        public IncluirEmailCaixaDeSaidaRequestHandler(IUnitOfWork unitOfWork, IEmailCaixaDeSaidaRepository emailCaixaDeSaidaRepository) : base(unitOfWork)
        {
            _emailCaixaDeSaidaRepository = emailCaixaDeSaidaRepository;
        }

        public async Task<Result> Handle(IncluirEmailCaixaDeSaidaRequest request, CancellationToken cancellationToken)
        {
            var email = Email.Criar(request.Email);
            if (email.IsFailure)
            {
                return email;
            }

            var emailCaixaDeSaida = EmailCaixaDeSaida.Criar(email.Value, request.Assunto, request.Template, request.MessageId);
            if (emailCaixaDeSaida.IsFailure)
            {
                return emailCaixaDeSaida;
            }

            await _emailCaixaDeSaidaRepository.IncluirAsync(emailCaixaDeSaida.Value);

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
