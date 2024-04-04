using CSharpFunctionalExtensions;

using Ineditta.Application.Documentos.Sindicais.Events.DocuemntosAprovados;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.EnviarEmailAprovacao
{
    public class EnviarEmailAprovacaoRequestHandler : BaseCommandHandler, IRequestHandler<EnviarEmailAprovacaoRequest, Result>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EnviarEmailAprovacaoRequestHandler(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher, IDocumentoSindicalRepository documentoSindicalRepository, IUsuarioRepository usuarioRepository) : base(unitOfWork)
        {
            _messagePublisher = messagePublisher;
            _documentoSindicalRepository = documentoSindicalRepository;
            _usuarioRepository = usuarioRepository;
        }
        public async Task<Result> Handle(EnviarEmailAprovacaoRequest request, CancellationToken cancellationToken)
        {
            const long idModuloConsultarDocumentos = 10;

            var documento = await _documentoSindicalRepository.ObterPorIdAsync(request.DocumentoId);

            if (documento == null)
            {
                return Result.Failure("O documento de id " + request.DocumentoId + " não foi encontrado");
            }

            if (!documento.DataAprovacao.HasValue)
            {
                return Result.Failure("O documento não está aprovado.");
            }

            if (documento.Origem == "Cliente Implantação" || documento.Origem == "Cliente Nova Negociação")
            {
                return Result.Failure("O tipo de origem do documento não permite notificação.");
            }

            var usuarios = await _usuarioRepository.ObterPorDocumentoId(request.DocumentoId, request.UsuariosIds);

            if (usuarios == null || !usuarios.Any())
            {
                return Result.Failure("Nenhum usuário a ser notificado");
            }

            usuarios = usuarios.Where(u =>
                u.NotificarEmail &&
                u.ModulosComerciais != null &&
                u.ModulosComerciais.Any(mc => mc.Id == idModuloConsultarDocumentos && mc.Consultar) &&
                !u.Bloqueado
            ).ToList();

            foreach (var usuario in usuarios)
            {
                DocumentoAprovadoEvent evento = new(
                    request.DocumentoId,
                    usuario.Id
                );

                await _messagePublisher.SendAsync(evento, cancellationToken);
            }

            return Result.Success();
        }
    }
}
