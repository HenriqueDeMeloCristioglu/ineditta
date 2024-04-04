using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.Documentos.Sindicais.Services
{
    public interface IDocumentoSindicalEmailService
    {
        ValueTask<Result<bool, Error>> EnviarEmail(IEnumerable<string> emails, string template, string assunto, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarEmailNotificacaoDocumentoCriadoAsync(string email, long documentoSindicalId, CancellationToken cancellationToken = default);
        ValueTask<Result<bool, Error>> EnviarNotificacaoDocumentoAprovadoAsync(DocumentoAprovadoEmailDto infoEmail, long documentoId, IEnumerable<string> emails, CancellationToken cancellationToken);
    }
}
