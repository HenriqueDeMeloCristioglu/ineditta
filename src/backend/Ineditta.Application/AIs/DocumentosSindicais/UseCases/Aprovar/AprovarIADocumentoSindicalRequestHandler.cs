using CSharpFunctionalExtensions;
using Ineditta.Application.Clausulas.Gerais.Entities;
using Ineditta.Application.Clausulas.Gerais.Repositiories;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.Usuarios.Factories;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using MediatR;
using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClausulasQuebradas;
using System.Linq;
using Ineditta.Application.AIs.Clausulas.Entities;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Aprovar
{
    public class AprovarIADocumentoSindicalRequestHandler : BaseCommandHandler, IRequestHandler<AprovarIADocumentoSindicalRequest, Result>
    {
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IIAClausulaRepository _iAClausulaRepository;
        private readonly IClausulaGeralRepository _clausulaGeralRepository;
        private readonly ObterUsuarioLogadoFactory _obterUsuarioLogadoFactory;
        private readonly IMessagePublisher _messagePublisher;

        public AprovarIADocumentoSindicalRequestHandler(IUnitOfWork unitOfWork,
                                                        IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                        IIAClausulaRepository iAClausulaRepository,
                                                        ObterUsuarioLogadoFactory obterUsuarioLogadoFactory,
                                                        IClausulaGeralRepository clausulaGeralRepository,
                                                        IMessagePublisher messagePublisher) : base(unitOfWork)
        {
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _iAClausulaRepository = iAClausulaRepository;
            _obterUsuarioLogadoFactory = obterUsuarioLogadoFactory;
            _clausulaGeralRepository = clausulaGeralRepository;
            _messagePublisher = messagePublisher;
        }

        public async Task<Result> Handle(AprovarIADocumentoSindicalRequest request, CancellationToken cancellationToken)
        {
            var usuario = await _obterUsuarioLogadoFactory.PorEmail();

            if (usuario.IsFailure)
            {
                return usuario;
            }

            var iaDocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(request.DocumentoId);
            if (iaDocumentoSindical == null) return Result.Failure("Nenhum ia documento sindical encontrado com o id: " + request.DocumentoId);

            var existeClausulaInconsistente = await _iAClausulaRepository.ExisteClausulaInconsistenteAsync(iaDocumentoSindical.Id);

            if (iaDocumentoSindical.Status != IADocumentoStatus.AguardandoAprovacaoQuebraClausula &&
                iaDocumentoSindical.Status != IADocumentoStatus.AguardandoAprovacaoClassificacao &&
                iaDocumentoSindical.Status != IADocumentoStatus.Erro)
            {
                return Result.Failure("Status atual não permite aprovação!");
            }

            if (iaDocumentoSindical.Status == IADocumentoStatus.Erro && existeClausulaInconsistente)
            {
                return Result.Failure("Documento com o status ERRO só pode ser aprovado se todas as cláusulas estiverem consistentes.");
            }

            if (iaDocumentoSindical.Status == IADocumentoStatus.AguardandoAprovacaoQuebraClausula)
            {
                iaDocumentoSindical.ClassificarClausula();

                await CommitAsync(cancellationToken);

                DocumentoSindicalClausulasQuebradasEvent message = new(iaDocumentoSindical.Id);
                await _messagePublisher.SendAsync(message, cancellationToken);

                return Result.Success();
            }

            var iaClausulas = await _iAClausulaRepository.ObterTodosPorIADocumentoIdAsync(iaDocumentoSindical.Id);

            if (iaClausulas == null)
            {
                return Result.Failure("Cláusulas não encontradas");
            }

            if (iaClausulas.Any(iaClausula => iaClausula.EstruturaClausulaId == null))
            {
                return Result.Failure("Cláusulas sem classificação não podem ser aprovadas");
            }

            if (iaClausulas.Any(iaClausula => iaClausula.SinonimoId == null))
            {
                return Result.Failure("Cláusulas sem sinônimos não podem ser aprovadas");
            }

            if (iaClausulas.Any(iaClausula => iaClausula.Status == IAClausulaStatus.Inconsistente))
            {
                return Result.Failure("Cláusulas com status Inconsistente não podem ser aprovadas");
            }

            if (usuario.Value.ModulosSISAP is null || !usuario.Value.ModulosSISAP.Any(modulo => modulo.Id == 70 && modulo.Aprovar))
            {
                return Result.Failure("Usuário não tem acesso para aprovar Documento");
            }

            iaDocumentoSindical.Aprovar(usuario.Value.Id);

            await _iADocumentoSindicalRepository.AtualizarAsync(iaDocumentoSindical);

            var iaClausulasIaDocumento = await _iAClausulaRepository.ObterTodosPorIADocumentoIdAsync(iaDocumentoSindical.Id);
            if (iaClausulasIaDocumento == null) return Result.Failure("Nenhuma ia_clausula encontrada para o ia_documento de id: " + iaDocumentoSindical.Id);

            foreach (var iaClausula in iaClausulasIaDocumento)
            {
                var criarClausulaResult = ClausulaGeral.Criar(
                        iaClausula.Texto,
                        iaDocumentoSindical.DocumentoReferenciaId,
                        iaClausula.EstruturaClausulaId ?? 0,
                        iaClausula.Numero,
                        null,
                        iaClausula.SinonimoId,
                        usuario.Value.Id,
                        null,
                        true,
                        true
                );

                if (criarClausulaResult.IsFailure)
                {
                    return criarClausulaResult;
                }

                criarClausulaResult.Value.Aprovar(usuario.Value.Id);

                await _clausulaGeralRepository.IncluirAsync(criarClausulaResult.Value);
            }

            await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
