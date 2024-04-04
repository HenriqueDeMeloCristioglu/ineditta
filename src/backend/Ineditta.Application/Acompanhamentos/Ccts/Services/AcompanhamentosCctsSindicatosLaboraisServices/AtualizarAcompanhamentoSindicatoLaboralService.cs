using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosLaborais.Repositories;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosLaboraisServices
{
    public class AtualizarAcompanhamentoSindicatoLaboralService
    {
        private readonly IAcompanhamentoCctSindicatoLaboralRepository _acompanhamentoCctSindicatoLaboralRepository;
        private readonly IncluirAcompanhamentoCctSindicatoLaboralService _incluirAcompanhamentoCctSindicatoLaboralService;
        private readonly DeletarAcompanhamentoSindicatoLaboralService _deletarAcompanhamentoSindicatoLaboralService;
        public AtualizarAcompanhamentoSindicatoLaboralService(IAcompanhamentoCctSindicatoLaboralRepository acompanhamentoCctSindicatoLaboralRepository, IncluirAcompanhamentoCctSindicatoLaboralService incluirAcompanhamentoCctSindicatoLaboralService, DeletarAcompanhamentoSindicatoLaboralService deletarAcompanhamentoSindicatoLaboralService)
        {
            _acompanhamentoCctSindicatoLaboralRepository = acompanhamentoCctSindicatoLaboralRepository;
            _incluirAcompanhamentoCctSindicatoLaboralService = incluirAcompanhamentoCctSindicatoLaboralService;
            _deletarAcompanhamentoSindicatoLaboralService = deletarAcompanhamentoSindicatoLaboralService;
        }

        public async Task<Result> Atualizar(long acompanhamentoCctId, IEnumerable<int>? sindicatosLaboraisIds, CancellationToken cancellationToken)
        {
            var acompanhamentosCctsSinditosLaborais = await _acompanhamentoCctSindicatoLaboralRepository.ObterPorAcompanhamentoIdAsync(acompanhamentoCctId);

            if (sindicatosLaboraisIds is not null && sindicatosLaboraisIds.Any())
            {
                List<int> idsLaboraisParaAdicionar = new();
                List<AcompanhamentoCctSinditoLaboral> acompanhamentosSindicatosLaboraisParaDeletar = new();

                if (acompanhamentosCctsSinditosLaborais is not null && acompanhamentosCctsSinditosLaborais.Any())
                {
                    var sindicatosParaDeletar = acompanhamentosCctsSinditosLaborais.Where(acsl => !sindicatosLaboraisIds.Any(slid => acsl.SindicatoId == slid)).ToList();

                    if (sindicatosParaDeletar is not null && sindicatosParaDeletar.Any())
                    {
                        acompanhamentosSindicatosLaboraisParaDeletar.AddRange(sindicatosParaDeletar);
                    }

                    var sindicatosParaAdicionar = sindicatosLaboraisIds.Where(slid => !acompanhamentosCctsSinditosLaborais.Any(acsl => slid == acsl.SindicatoId)).ToList();
                    idsLaboraisParaAdicionar.AddRange(sindicatosParaAdicionar);

                    var resultIncluirSindicatosLaborais = await _incluirAcompanhamentoCctSindicatoLaboralService.Incluir(idsLaboraisParaAdicionar, acompanhamentoCctId, cancellationToken);
                    if (resultIncluirSindicatosLaborais.IsFailure)
                    {
                        return resultIncluirSindicatosLaborais;
                    }

                    var resultDeletarAcompanhamentoCctSindicatoLaboral = await _deletarAcompanhamentoSindicatoLaboralService.Deletar(acompanhamentosSindicatosLaboraisParaDeletar, cancellationToken);

                    if (resultDeletarAcompanhamentoCctSindicatoLaboral.IsFailure)
                    {
                        return resultDeletarAcompanhamentoCctSindicatoLaboral;
                    }
                }
                else
                {
                    var resultIncluirSindicatosLaborais = await _incluirAcompanhamentoCctSindicatoLaboralService.Incluir(sindicatosLaboraisIds, acompanhamentoCctId, cancellationToken);

                    if (resultIncluirSindicatosLaborais.IsFailure)
                    {
                        return resultIncluirSindicatosLaborais;
                    }
                }
            }
            else
            {
                if (acompanhamentosCctsSinditosLaborais is not null && acompanhamentosCctsSinditosLaborais.Any())
                {
                    var resultDeletarAcompanhamentoCctSindicatoLaboral = await _deletarAcompanhamentoSindicatoLaboralService.Deletar(acompanhamentosCctsSinditosLaborais, cancellationToken);

                    if (resultDeletarAcompanhamentoCctSindicatoLaboral.IsFailure)
                    {
                        return resultDeletarAcompanhamentoCctSindicatoLaboral;
                    }
                }
            }

            return Result.Success();
        }
    }
}
