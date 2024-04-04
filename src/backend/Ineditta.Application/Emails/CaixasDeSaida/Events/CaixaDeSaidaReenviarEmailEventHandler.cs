using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.Integration.Email.Dtos;
using Ineditta.Integration.Email.Protocols;

using Microsoft.Extensions.Logging;

namespace Ineditta.Application.Emails.CaixasDeSaida.Events
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class CaixaDeSaidaReenviarEmailEventHandler : IRequestHandler<CaixaDeSaidaReenviarEmailEvent>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<CaixaDeSaidaReenviarEmailEventHandler> _logger;
        private readonly IEmailCaixaDeSaidaRepository _emailCaixaDeSaidaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CaixaDeSaidaReenviarEmailEventHandler(IUnitOfWork unitOfWork, IEmailSender emailSender, ILogger<CaixaDeSaidaReenviarEmailEventHandler> logger, IEmailCaixaDeSaidaRepository emailCaixaDeSaidaRepository)
        {
            _emailSender = emailSender;
            _logger = logger;
            _emailCaixaDeSaidaRepository = emailCaixaDeSaidaRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<Result> Handle(CaixaDeSaidaReenviarEmailEvent message, CancellationToken cancellationToken = default)
        {
            foreach (var item in message.Emails)
            {
                await _emailCaixaDeSaidaRepository.DeletarAsync(item);

                await _unitOfWork.CommitAsync(cancellationToken);

                var sendEmailRequestDto = new SendEmailRequestDto(new List<string>() { item.Email.Valor }, item.Template, item.Assunto);

                var result = await _emailSender.SendAsync(sendEmailRequestDto, cancellationToken);

                if (result.IsFailure)
                {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
                    _logger.LogInformation($"Erro ao enviar email para {result.Error.Message}");
#pragma warning restore CA2254 // Template should be a static expression
#pragma warning restore CA1848 // Use the LoggerMessage delegates
                }
            }

            return Result.Success();
        }
    }
}
