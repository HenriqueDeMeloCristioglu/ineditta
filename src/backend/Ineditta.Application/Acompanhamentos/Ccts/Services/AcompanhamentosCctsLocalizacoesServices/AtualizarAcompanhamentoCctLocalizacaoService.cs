using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Entities;
using Ineditta.Application.Acompanhamentos.CctsLocalizacoes.Repositories;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsLocalizacoesServices
{
    public class AtualizarAcompanhamentoCctLocalizacaoService
    {
        private readonly IAcompanhamentoCctLocalizacaoRepository _acompanhamentoCctLocalizacaoRepository;
        private readonly IncluirAcompanhamentoCctLocalizacaoService _incluirAcompanhamentoCctLocalizacaoService;
        private readonly DeletarAcompanhamentoCctLocalizacaoService _deletarAcompanhamentoCctLocalizacaoService;
        public AtualizarAcompanhamentoCctLocalizacaoService(IAcompanhamentoCctLocalizacaoRepository acompanhamentoCctLocalizacaoRepository, IncluirAcompanhamentoCctLocalizacaoService incluirAcompanhamentoCctLocalizacaoService, DeletarAcompanhamentoCctLocalizacaoService deletarAcompanhamentoCctLocalizacaoService)
        {
            _acompanhamentoCctLocalizacaoRepository = acompanhamentoCctLocalizacaoRepository;
            _incluirAcompanhamentoCctLocalizacaoService = incluirAcompanhamentoCctLocalizacaoService;
            _deletarAcompanhamentoCctLocalizacaoService = deletarAcompanhamentoCctLocalizacaoService;
        }

        public async Task<Result> Atualizar(long acompanhamentoCctId, IEnumerable<int>? localizacoesIds, CancellationToken cancellationToken)
        {
            var acompanhamentosCctsLocalizacoes = await _acompanhamentoCctLocalizacaoRepository.ObterPorAcompanhamentoIdAsync(acompanhamentoCctId);

            if (localizacoesIds is not null && localizacoesIds.Any())
            {

                List<int> idsLocalizacoesParaAdicionar = new();
                List<AcompanhamentoCctLocalizacao> acompanhamentosLocalizacoesParaDeletar = new();

                if (acompanhamentosCctsLocalizacoes is not null && acompanhamentosCctsLocalizacoes.Any())
                {
                    var localizacoesParaDeletar = acompanhamentosCctsLocalizacoes.Where(acsl => !localizacoesIds.Any(slid => acsl.LocalizacaoId == slid)).ToList();

                    if (localizacoesParaDeletar is not null && localizacoesParaDeletar.Any())
                    {
                        acompanhamentosLocalizacoesParaDeletar.AddRange(localizacoesParaDeletar);
                    }

                    var localizacoesParaAdicionar = localizacoesIds.Where(slid => !acompanhamentosCctsLocalizacoes.Any(acsl => slid == acsl.LocalizacaoId)).ToList();

                    idsLocalizacoesParaAdicionar.AddRange(localizacoesParaAdicionar);

                    var resultIncluirLocalizacoes = await _incluirAcompanhamentoCctLocalizacaoService.Incluir(idsLocalizacoesParaAdicionar, acompanhamentoCctId, cancellationToken);

                    if (resultIncluirLocalizacoes.IsFailure)
                    {
                        return resultIncluirLocalizacoes;
                    }

                    var resultDeletarAcompanhamentoCctLocalizacoes = await _deletarAcompanhamentoCctLocalizacaoService.Deletar(acompanhamentosLocalizacoesParaDeletar, cancellationToken);

                    if (resultDeletarAcompanhamentoCctLocalizacoes.IsFailure)
                    {
                        return resultDeletarAcompanhamentoCctLocalizacoes;
                    }
                }
                else
                {
                    var resultIncluirLocalizacoes = await _incluirAcompanhamentoCctLocalizacaoService.Incluir(localizacoesIds, acompanhamentoCctId, cancellationToken);

                    if (resultIncluirLocalizacoes.IsFailure)
                    {
                        return resultIncluirLocalizacoes;
                    }
                }
            }
            else
            {
                if (acompanhamentosCctsLocalizacoes is not null && acompanhamentosCctsLocalizacoes.Any())
                {
                    var resultDeletarAcompanhamentoCctLocalizacoes = await _deletarAcompanhamentoCctLocalizacaoService.Deletar(acompanhamentosCctsLocalizacoes, cancellationToken);

                    if (resultDeletarAcompanhamentoCctLocalizacoes.IsFailure)
                    {
                        return resultDeletarAcompanhamentoCctLocalizacoes;
                    }
                }
            }

            return Result.Success();
        }
    }
}
