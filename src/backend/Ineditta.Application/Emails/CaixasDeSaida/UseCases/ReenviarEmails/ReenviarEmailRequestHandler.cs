using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Repositories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.Integration.Email.Dtos;
using Ineditta.Integration.Email.Protocols;

using MediatR;

namespace Ineditta.Application.Emails.CaixasDeSaida.UseCases.ReenviarEmails
{
    public class ReenviarEmailRequestHandler : BaseCommandHandler, IRequestHandler<ReenviarEmailRequest, Result>
    {
        private readonly IEmailCaixaDeSaidaRepository _emailCaixaDeSaidaRepository;
        private readonly IEmailSender _emailSender;

        public ReenviarEmailRequestHandler(IUnitOfWork unitOfWork, IEmailCaixaDeSaidaRepository emailCaixaDeSaidaRepository, IEmailSender emailSender) : base(unitOfWork)
        {
            _emailCaixaDeSaidaRepository = emailCaixaDeSaidaRepository;
            _emailSender = emailSender;
        }

        public async Task<Result> Handle(ReenviarEmailRequest request, CancellationToken cancellationToken)
        {
            var emails = await _emailCaixaDeSaidaRepository.ObterTodosAsync();

            if (emails.IsFailure)
            {
                return emails;
            }

            foreach (var item in emails.Value)
            {
                await _emailCaixaDeSaidaRepository.DeletarAsync(item);

                await CommitAsync(cancellationToken);

                var sendEmailRequestDto = new SendEmailRequestDto(new List<string>() { item.Email.Valor }, item.Assunto, item.Template);

                var result = await _emailSender.SendAsync(sendEmailRequestDto, cancellationToken);

                if (result.IsFailure)
                {
                    return Result.Failure($"Não foi possível enviar email {item.Email}");
                }
            }

            return Result.Success();
        }
    }
}
