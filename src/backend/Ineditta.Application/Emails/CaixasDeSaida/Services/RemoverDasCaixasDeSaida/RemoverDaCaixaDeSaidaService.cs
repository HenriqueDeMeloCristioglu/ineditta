using CSharpFunctionalExtensions;

using Ineditta.Application.Emails.CaixasDeSaida.Repositories;
using Ineditta.BuildingBlocks.Core.Database;

namespace Ineditta.Application.Emails.StoragesManagers.Services.LimparCaixasDeSaida
{
    public class RemoverDaCaixaDeSaidaService : IRemoverDaCaixaDeSaidaService
    {
        private readonly IEmailCaixaDeSaidaRepository _emailCaixaDeSaidaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoverDaCaixaDeSaidaService(IUnitOfWork unitOfWork, IEmailCaixaDeSaidaRepository emailCaixaDeSaidaRepository)
        {
            _unitOfWork = unitOfWork;
            _emailCaixaDeSaidaRepository = emailCaixaDeSaidaRepository;
        }

        public async ValueTask<Result> RemoverAsync(string messageId, CancellationToken cancellationToken)
        {
            var emailCaixaDeSaidaRepository = await _emailCaixaDeSaidaRepository.ObterPorMessageId(messageId);

            if (emailCaixaDeSaidaRepository.IsFailure)
            {
                return Result.Failure("Erro ao encontrar Email na caixa de saída");
            }

            foreach (var emailItem in emailCaixaDeSaidaRepository.Value)
            {
                var result = await _emailCaixaDeSaidaRepository.DeletarAsync(emailItem);

                if (result.IsFailure)
                {
                    return Result.Failure("Erro ao Deletar Email da caixa de saída");
                }

                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return Result.Success();
        }
    }
}
