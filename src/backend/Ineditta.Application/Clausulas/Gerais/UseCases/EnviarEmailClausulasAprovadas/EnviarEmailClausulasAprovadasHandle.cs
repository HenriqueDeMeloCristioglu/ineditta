using CSharpFunctionalExtensions;

using Ineditta.Application.Clausulas.Gerais.Events.ClausulasAprovadasEmails;
using Ineditta.Application.Documentos.Sindicais.Repositories;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.Models;

using MediatR;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.EnviarEmailClausulasAprovadas
{
    public class EnviarEmailClausulasAprovadasHandle : BaseCommandHandler, IRequestHandler<EnviarEmailClausulasAprovadasRequest, IResult<Unit, Error>>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly IDocumentoSindicalRepository _documentoSindicalRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EnviarEmailClausulasAprovadasHandle(IUnitOfWork unitOfWork, IMessagePublisher messagePublisher, IDocumentoSindicalRepository documentoSindicalRepository, IUsuarioRepository usuarioRepository) : base(unitOfWork)
        {
            _messagePublisher = messagePublisher;
            _documentoSindicalRepository = documentoSindicalRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IResult<Unit, Error>> Handle(EnviarEmailClausulasAprovadasRequest request, CancellationToken cancellationToken)
        {
            const long idModuloClausulas = 6;

            var documento = await _documentoSindicalRepository.ObterPorIdAsync(request.DocumentoId);
            if (documento == null) return Result.Failure<Unit, Error>(Error.Create("Not Found", "O documento de id " + request.DocumentoId + " não foi encontrado"));
            if (!documento.DataAprovacao.HasValue) return Result.Failure<Unit, Error>(Error.Create("Business", "O documento não está aprovado."));

            if (documento.Origem == "Cliente Implantação" || documento.Origem == "Cliente Nova Negociação")
            {
                return Result.Failure<Unit, Error>(Error.Create("Business", "O tipo de origem do documento não permite notificação."));
            }

            var usuarios = await _usuarioRepository.ObterPorDocumentoId(request.DocumentoId, request.UsuariosIds);
            if (usuarios == null || !usuarios.Any())
            {
                return Result.Failure<Unit, Error>(Error.Create("Not Found", "Nenhum usuário a ser notificado"));
            }

            usuarios = usuarios.Where(u => 
                u.NotificarEmail &&
                u.ModulosComerciais != null &&
                u.ModulosComerciais.Any(mc => mc.Id == idModuloClausulas && mc.Consultar) && 
                !u.Bloqueado
            ).ToList();

            foreach (var usuario in usuarios)
            {
                ClausulasAprovadasEmailsEvent evento = new(
                    request.DocumentoId,
                    usuario.Id
                );

                await _messagePublisher.SendAsync(evento, cancellationToken);
            }

            return Result.Success<Unit, Error>(Unit.Value);
        }
    }
}
