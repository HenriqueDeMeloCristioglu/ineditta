using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;
using Ineditta.Application.Federacoes.UseCases.Upsert;
using Ineditta.Application.IndicadoresEconomicos.Entities;
using Ineditta.Application.IndicadoresEconomicos.Repositories;

using MediatR;

namespace Ineditta.Application.IndicadoresEconomicos.UseCases.Upsert
{
    public class UpsertIndicadorEconomicoRequestHandler : IRequestHandler<UpsertIndicadorEconomicoRequest, Result>
    {
        private readonly IIndicadorEconomicoRepository _indicadorEconomicoRepository;
        public UpsertIndicadorEconomicoRequestHandler(IIndicadorEconomicoRepository indicadorEconomicoRepository)
        {
            _indicadorEconomicoRepository = indicadorEconomicoRepository;
        }
        public async Task<Result> Handle(UpsertIndicadorEconomicoRequest request, CancellationToken cancellationToken)
        {
            var result = await _indicadorEconomicoRepository.ExisteAsync(request.Id);

            return !result ? await Incluir(request) : await Atualizar(request);
        }
        private async ValueTask<Result> Incluir(UpsertIndicadorEconomicoRequest request)
        {
            foreach (var periodoProjetado in request.PeriodosProjetados)
            {
                var indicadorEconomico = IndicadorEconomico.Criar(request.Origem, request.Indicador, request.IdUsuario, request.Fonte, periodoProjetado.Periodo, periodoProjetado.Projetado, DateTime.Now);

                if (indicadorEconomico.IsFailure)
                {
                    return Result.Failure(indicadorEconomico.Error);
                }

                await _indicadorEconomicoRepository.IncluirAsync(indicadorEconomico.Value);
            }

            return Result.Success("Indicador econômico incluido com sucesso");
        }

        private async ValueTask<Result> Atualizar(UpsertIndicadorEconomicoRequest request)
        {
            var indicadorEconomico = await _indicadorEconomicoRepository.ObterAsync(request.Id);

            if (indicadorEconomico == null)
            {
                return Result.Failure("Indicador econômico não foi encontrado");
            }

            foreach (var periodoProjetado in request.PeriodosProjetados)
            {
                var result = indicadorEconomico.Atualizar(request.Origem, request.Indicador, request.IdUsuario, request.Fonte, periodoProjetado.Periodo, periodoProjetado.Projetado);

                if (result.IsFailure)
                {
                    return Result.Failure(result.Error);
                }

                await _indicadorEconomicoRepository.AtualizarAsync(indicadorEconomico);
            }

            return Result.Success("Indicador econômico atualizado com sucesso");
        }
    }
}
