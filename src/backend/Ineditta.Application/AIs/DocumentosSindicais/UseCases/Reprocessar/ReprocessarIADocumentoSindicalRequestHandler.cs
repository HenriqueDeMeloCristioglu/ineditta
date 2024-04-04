using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClausulasQuebradas;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada.ClassificacaoReprocessada;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalQuebraClausulaIniciada.QuebraClausulaReprocessada;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Extensions;

using MediatR;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Reprocessar
{
    public class ReprocessarIADocumentoSindicalRequestHandler : BaseCommandHandler, IRequestHandler<ReprocessarIADocumentoSindicalRequest, Result>
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;
        private readonly IMessagePublisher _messagePublisher;

        public ReprocessarIADocumentoSindicalRequestHandler(IUnitOfWork unitOfWork,
                                                        IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                        ObterUsuarioLogadoFactory obterUsuarioLogadoFactory,
                                                        IMessagePublisher messagePublisher) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
            _messagePublisher = messagePublisher;
        }

        public async Task<Result> Handle(ReprocessarIADocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _obterUsuarioLogadoFactory.PorEmail();

            if (usuario.IsFailure)
            {
                return usuario;
            }

            var iaDocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(request.DocumentoId);
            if (iaDocumentoSindical == null) return Result.Failure("Nenhum ia documento sindical encontrado com o id: " + request.DocumentoId);

            if (iaDocumentoSindical.Status != IADocumentoStatus.Erro)
            {
                return Result.Failure("Status atual não permite reprocessamento!");
            }

            if (iaDocumentoSindical.Status == IADocumentoStatus.AguardandoAprovacaoQuebraClausula)
            {
                iaDocumentoSindical.ClassificarClausula();

                await CommitAsync(cancellationToken);

                DocumentoSindicalClausulasQuebradasEvent message = new(iaDocumentoSindical.Id);
                await _messagePublisher.SendAsync(message, cancellationToken);

                return Result.Success();
            }

            if (usuario.Value.ModulosSISAP is null)
            {
                return Result.Failure("Usuário não tem acesso para reprocessar Documento");
            }

            if (iaDocumentoSindical.UltimoStatusProcessado != IADocumentoStatus.QuebrandoClausulas && iaDocumentoSindical.UltimoStatusProcessado != IADocumentoStatus.ClassificandoClausulas)
            {
                return Result.Failure($"Último status processado não permite reprocessamento: {iaDocumentoSindical.UltimoStatusProcessado.GetDescription()}");
            }

            iaDocumentoSindical.Reprocessar();

            await _iADocumentoSindicalRepository.AtualizarAsync(iaDocumentoSindical);

            await CommitAsync(cancellationToken);

            if (iaDocumentoSindical.UltimoStatusProcessado == IADocumentoStatus.QuebrandoClausulas)
            {
                QuebraClausulaReprocessadaEvent evento = new(iaDocumentoSindical.Id);
                await _messagePublisher.SendAsync(evento, cancellationToken);
            }
            else if (iaDocumentoSindical.UltimoStatusProcessado == IADocumentoStatus.ClassificandoClausulas)
            {
                ClassificacaoReprocessadaEvent evento = new(iaDocumentoSindical.Id);
                await _messagePublisher.SendAsync(evento, cancellationToken);
            }

            return Result.Success();
        }
    }
}
