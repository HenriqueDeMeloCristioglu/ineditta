using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Documentos.Sindicais.Services;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.Application.Documentos.Sindicais.Events.DocumentoCriado
{
#pragma warning disable CA1711 // Identificadores não devem ter um sufixo incorreto
    public class DocumentoCriadoEventHandler : IRequestHandler<DocumentoCriadoEvent>
#pragma warning restore CA1711 // Identificadores não devem ter um sufixo incorreto
    {
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IDocumentoSindicalEmailService _documentoSindicalEmailService;
        private readonly IUsuarioRepository _usuarioRepository;
        public DocumentoCriadoEventHandler(IDocumentoSindicalRepository documentoSindicalRepository, IDocumentoSindicalEmailService documentoSindicalEmailService, IUsuarioRepository usuarioRepository)
        {
            _documentoSindicalRepository = documentoSindicalRepository;
            _documentoSindicalEmailService = documentoSindicalEmailService;
            _usuarioRepository = usuarioRepository;
        }

        public async ValueTask<Result> Handle(DocumentoCriadoEvent message, CancellationToken cancellationToken = default)
        {
            var documentoSindical = await _documentoSindicalRepository.ObterPorIdAsync(message.DocumentoSindicalId);

            if (documentoSindical is null)
            {
                throw new ArgumentException("Documento sindical não foi encontrado");
            }

            var usuario = await _usuarioRepository.ObterPorIdAsync(message.UsuarioId);

            if (usuario == null)
            {
                throw new ArgumentException("Nenhum usuário para notificação encontrado");
            }

            var resultEmailSender = await _documentoSindicalEmailService.EnviarEmailNotificacaoDocumentoCriadoAsync(usuario.Email.Valor, documentoSindical.Id, cancellationToken);
            if (resultEmailSender.IsFailure)
            {
                throw new ArgumentException("Algo deu errado ao tentar enviar o email. Erro: " + resultEmailSender.Error.Message);
            }

            return Result.Success();
        }
    }
}
