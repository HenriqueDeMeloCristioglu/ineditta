using CSharpFunctionalExtensions;

using Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosPatronaisServices;
using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Entities;
using Ineditta.Application.Acompanhamentos.CctsSindicatosPatronais.Repositories;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsSindicatosLaboraisServices
{
    public class AtualizarAcompanhamentoCctSindicatoPatronalService
    {
        private readonly IAcompanhamentoCctSindicatoPatronalRepository _acompanhamentoCctSindicatoPatronalRepository;
        private readonly IncluirAcompanhamentoCctSindicatoPatronalService _incluirAcompanhamentoCctSindicatoPatronalService;
        private readonly DeletarAcompanhamentoSindicatoPatronalService _deletarAcompanhamentoSindicatoPatronalService;
        public AtualizarAcompanhamentoCctSindicatoPatronalService(IAcompanhamentoCctSindicatoPatronalRepository acompanhamentoCctSindicatoPatronalRepository, IncluirAcompanhamentoCctSindicatoPatronalService incluirAcompanhamentoCctSindicatoPatronalService, DeletarAcompanhamentoSindicatoPatronalService deletarAcompanhamentoSindicatoPatronalService)
        {
            _acompanhamentoCctSindicatoPatronalRepository = acompanhamentoCctSindicatoPatronalRepository;
            _incluirAcompanhamentoCctSindicatoPatronalService = incluirAcompanhamentoCctSindicatoPatronalService;
            _deletarAcompanhamentoSindicatoPatronalService = deletarAcompanhamentoSindicatoPatronalService;
        }

        public async Task<Result> Atualizar(long acompanhamentoCctId, IEnumerable<int>? sindicatosPatronaisIds, CancellationToken cancellationToken)
        {
            var acompanhamentosCctsSinditosPatronais = await _acompanhamentoCctSindicatoPatronalRepository.ObterPorAcompanhamentoIdAsync(acompanhamentoCctId);

            if (sindicatosPatronaisIds is not null && sindicatosPatronaisIds.Any())
            {
                List<int> idsPatronaisParaAdicionar = new();
                List<AcompanhamentoCctSinditoPatronal> acompanhamentosSindicatosPatronaisParaDeletar = new();

                if (acompanhamentosCctsSinditosPatronais is not null && acompanhamentosCctsSinditosPatronais.Any())
                {
                    var sindicatosParaDeletar = acompanhamentosCctsSinditosPatronais.Where(acsl => !sindicatosPatronaisIds.Any(slid => acsl.SindicatoId == slid)).ToList();

                    if (sindicatosParaDeletar is not null && sindicatosParaDeletar.Any())
                    {
                        acompanhamentosSindicatosPatronaisParaDeletar.AddRange(sindicatosParaDeletar);
                    }

                    var sindicatosParaAdicionar = sindicatosPatronaisIds.Where(slid => !acompanhamentosCctsSinditosPatronais.Any(acsl => slid == acsl.SindicatoId)).ToList();

                    idsPatronaisParaAdicionar.AddRange(sindicatosParaAdicionar);
                    var resultIncluirSindicatosPatronais = await _incluirAcompanhamentoCctSindicatoPatronalService.Incluir(idsPatronaisParaAdicionar, acompanhamentoCctId, cancellationToken);

                    if (resultIncluirSindicatosPatronais.IsFailure)
                    {
                        return resultIncluirSindicatosPatronais;
                    }

                    var resultDeletarAcompanhamentoCctSindicatoPatronal = await _deletarAcompanhamentoSindicatoPatronalService.Deletar(acompanhamentosSindicatosPatronaisParaDeletar, cancellationToken);

                    if (resultDeletarAcompanhamentoCctSindicatoPatronal.IsFailure)
                    {
                        return resultDeletarAcompanhamentoCctSindicatoPatronal;
                    }
                }
                else
                {
                    if (sindicatosPatronaisIds is not null && sindicatosPatronaisIds.Any())
                    {
                        var resultIncluirSindicatosPatronais = await _incluirAcompanhamentoCctSindicatoPatronalService.Incluir(sindicatosPatronaisIds, acompanhamentoCctId, cancellationToken);

                        if (resultIncluirSindicatosPatronais.IsFailure)
                        {
                            return resultIncluirSindicatosPatronais;
                        }
                    }
                }
            }
            else
            {
                if (acompanhamentosCctsSinditosPatronais is not null && acompanhamentosCctsSinditosPatronais.Any())
                {

                    var resultDeletarAcompanhamentoCctSindicatoPatronal = await _deletarAcompanhamentoSindicatoPatronalService.Deletar(acompanhamentosCctsSinditosPatronais, cancellationToken);

                    if (resultDeletarAcompanhamentoCctSindicatoPatronal.IsFailure)
                    {
                        return resultDeletarAcompanhamentoCctSindicatoPatronal;
                    }
                }
            }

            return Result.Success();
        }
    }
}
