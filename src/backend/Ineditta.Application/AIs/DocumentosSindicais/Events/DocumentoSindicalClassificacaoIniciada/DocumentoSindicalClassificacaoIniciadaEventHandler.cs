using CSharpFunctionalExtensions;

using Ineditta.Application.AIs.Clausulas.Entities;
using Ineditta.Application.AIs.Clausulas.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Dtos;
using Ineditta.Application.AIs.DocumentosSindicais.Entities;
using Ineditta.Application.AIs.DocumentosSindicais.Repositories;
using Ineditta.Application.AIs.DocumentosSindicais.Services.ClassificacaoClausula;
using Ineditta.Application.EstruturasClausulas.Gerais.Entities;
using Ineditta.Application.EstruturasClausulas.Gerais.Repositories;
using Ineditta.Application.Sinonimos.Entities;
using Ineditta.Application.Sinonimos.Repositories;
using Ineditta.BuildingBlocks.Core.Bus;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using Microsoft.Extensions.Logging;

namespace Ineditta.Application.AIs.DocumentosSindicais.Events.DocumentoSindicalClassificacaoIniciada
{
#pragma warning disable CA1711 // Identificadores não devem ter um sufixo incorreto
    public class DocumentoSindicalClassificacaoIniciadaEventHandler : BaseCommandHandler, IRequestHandler<DocumentoSindicalClassificacaoIniciadaEvent>
#pragma warning restore CA1711 // Identificadores não devem ter um sufixo incorreto
    {
        private readonly IClassificacaoClausulaService _classificacaoClausulaService;
        private readonly IIADocumentoSindicalRepository _iADocumentoSindicalRepository;
        private readonly IIAClausulaRepository _iAClausulaRepository;
        private readonly ISinonimoRepository _sinonimoRepository;
        private readonly IEstruturaClausulaRepository _estruturaClausulaRepository;
        private readonly ILogger<DocumentoSindicalClassificacaoIniciadaEventHandler> _logger;

        public DocumentoSindicalClassificacaoIniciadaEventHandler(IUnitOfWork unitOfWork, 
                                                                  IClassificacaoClausulaService classificacaoClausulaService,
                                                                  IIADocumentoSindicalRepository iADocumentoSindicalRepository,
                                                                  IIAClausulaRepository iAClausulaRepository,
                                                                  ISinonimoRepository sinonimoRepository,
                                                                  IEstruturaClausulaRepository estruturaClausulaRepository,
                                                                  ILogger<DocumentoSindicalClassificacaoIniciadaEventHandler> logger) : base(unitOfWork)
        {
            _classificacaoClausulaService = classificacaoClausulaService;
            _iADocumentoSindicalRepository = iADocumentoSindicalRepository;
            _iAClausulaRepository = iAClausulaRepository;
            _sinonimoRepository = sinonimoRepository;
            _estruturaClausulaRepository = estruturaClausulaRepository;
            _logger = logger;
        }

        public async ValueTask<Result> Handle(DocumentoSindicalClassificacaoIniciadaEvent message, CancellationToken cancellationToken = default)
        {
            var iAClausula = await _iAClausulaRepository.ObterPorIdAsync(message.IAClausulaId);

            if (iAClausula is null)
            {
                _logger.LogError("Nenhuma cláusula vinculada ao documento sindical foi encontrada");
                return Result.Failure("Nenhuma cláusula vinculada ao documento sindical foi encontrada");
            }

            var iADocumentoSindical = await _iADocumentoSindicalRepository.ObterPorIdAsync(iAClausula.IADocumentoSindicalId);

            if (iADocumentoSindical is null)
            {
                _logger.LogError("Documento sindical IA não foi encontrado");
                return Result.Failure("Documento sindical IA não foi encontrado");
            }

            var (sinonimo, estruturaClausula) = await ObterSinonimoEstruturaClausulaAsync(iAClausula.Nome);

            if (sinonimo is null || estruturaClausula is null)
            {
                var classificacaoComIA = await ClassificarClausulaComIAAsync(iAClausula);

                if (classificacaoComIA.IsFailure)
                {
                    _logger.LogError("Erro ao classifica a cláusula com a IA: {Error}", classificacaoComIA.Error);
                    iADocumentoSindical.RegistrarErro($"Erro ao classifica a cláusula com a IA: {classificacaoComIA.Error}");
                } else
                {
                    sinonimo = classificacaoComIA.Value.Item1;
                    estruturaClausula = classificacaoComIA.Value.Item2;
                }
            }

            var classificacao = iAClausula.Classificar(estruturaClausula?.Id, sinonimo?.Id, ((sinonimo?.Id ?? 0) > 0 && estruturaClausula?.Id > 0) ? IAClausulaStatus.Consistente : IAClausulaStatus.Inconsistente);

            if (classificacao.IsFailure)
            {
                _logger.LogError("Erro ao classificar: {Error}", classificacao.Error);
                iADocumentoSindical.RegistrarErro($"Erro ao classificar: {classificacao.Error}");
            }

            await _iAClausulaRepository.AtualizarAsync(iAClausula);

            if (iADocumentoSindical.Erro())
            {
                await _iADocumentoSindicalRepository.AtualizarAsync(iADocumentoSindical);
            }

            await CommitAsync(cancellationToken);

            await ConfirmarClassificacaoAsync(iADocumentoSindical, iAClausula, cancellationToken);

            return Result.Success();
        }

        private async Task<(Sinonimo?, EstruturaClausula?)> ObterSinonimoEstruturaClausulaAsync(string? nome)
        {
            var sinonimo = await _sinonimoRepository.ObterPorNomeExatoAsync(nome ?? string.Empty) ??
                           await _sinonimoRepository.ObterPorNomeAproximadoAsync(nome ?? string.Empty);

            if (sinonimo is null)
                return (null, null);

            var estruturaClausula = await _estruturaClausulaRepository.ObterPorIdAsync(sinonimo.EstruturaClausulaId);

            if (estruturaClausula is null)
            {
                _logger.LogError("Erro ao localizar a estrutura cláusula.");
                return (sinonimo, null);
            }

            return (sinonimo, estruturaClausula);
        }

        private async Task<Result<(Sinonimo?, EstruturaClausula?)>> ClassificarClausulaComIAAsync(IAClausula iAClausula)
        {
            var result = await _classificacaoClausulaService.ClassificarClausula(new ClausulaDto(iAClausula.Grupo, iAClausula.SubGrupo, iAClausula.Numero, iAClausula.Nome ?? string.Empty, iAClausula.Texto));

            if (result.IsFailure)
            {
                _logger.LogError("Erro ao classificar: {Error}", result.Error.Message);
                return Result.Failure<(Sinonimo?, EstruturaClausula?)>($"Erro ao classificar: {result.Error.Message}");
            }

            var estruturaClausula = await _estruturaClausulaRepository.ObterPorIdAsync(result.Value.Classificacao);

            if (estruturaClausula is null)
            {
                _logger.LogError("Erro ao localizar a estrutura cláusula: {Error}", result.Value.Classificacao);
                return Result.Failure<(Sinonimo?, EstruturaClausula?)>($"Erro ao localizar a estrutura cláusula {result.Value.Classificacao}.");
            }

            var obterSinonimo = await _sinonimoRepository.ObterPorEstruturaClausulaIdENomeAsync(result.Value.Classificacao, iAClausula.Nome ?? string.Empty);

            return Result.Success<(Sinonimo?, EstruturaClausula?)>((obterSinonimo, estruturaClausula));
        }

        private async Task<IADocumentoSindical> ConfirmarClassificacaoAsync(IADocumentoSindical iADocumentoSindical, IAClausula iAClausula, CancellationToken cancellationToken)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _iADocumentoSindicalRepository.LockAsync(iADocumentoSindical.Id, cancellationToken);

                var existeClausulaPendenteClassificacao = await _iAClausulaRepository.ExisteClausulaPendenteClassificacaoAsync(iAClausula.IADocumentoSindicalId, iAClausula.Id);

                if (!existeClausulaPendenteClassificacao)
                {
                    iADocumentoSindical.ConfirmarClassificacaoClausula();

                    await _iADocumentoSindicalRepository.AtualizarAsync(iADocumentoSindical);
                }

                await CommitAsync(cancellationToken);

                return Result.Success();
            }, cancellationToken);

            return iADocumentoSindical;
        }
    }
}
