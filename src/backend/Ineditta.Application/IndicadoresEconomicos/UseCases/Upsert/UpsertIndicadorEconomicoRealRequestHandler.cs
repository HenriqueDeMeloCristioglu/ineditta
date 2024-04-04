using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;
using Ineditta.Application.Federacoes.UseCases.Upsert;
using Ineditta.Application.IndicadoresEconomicos.Entities;
using Ineditta.Application.IndicadoresEconomicos.Repositories;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace Ineditta.Application.IndicadoresEconomicos.UseCases.Upsert
{
    public class UpsertIndicadorEconomicoRealRequestHandler : IRequestHandler<UpsertIndicadorEconomicoRealRequest, Result>
    {
        private readonly IIndicadorEconomicoRealRepository _indicadorEconomicoRealRepository;
        public UpsertIndicadorEconomicoRealRequestHandler(IIndicadorEconomicoRealRepository indicadorEconomicoRealRepository)
        {
            _indicadorEconomicoRealRepository = indicadorEconomicoRealRepository;
        }
        public async Task<Result> Handle(UpsertIndicadorEconomicoRealRequest request, CancellationToken cancellationToken)
        {
            var result = await _indicadorEconomicoRealRepository.ExisteAsync(request.Id);

            return !result ? await Incluir(request) : await Atualizar(request);
        }
        private async ValueTask<Result> Incluir(UpsertIndicadorEconomicoRealRequest request)
        {
            foreach (var periodoReal in request.PeriodosReais)
            {
                var indicadorEconomico = IndicadorEconomicoReal.Criar(request.Indicador, periodoReal.Periodo, periodoReal.Real, DateTime.Now);

                if (indicadorEconomico.IsFailure)
                {
                    return Result.Failure(indicadorEconomico.Error);
                }

                await _indicadorEconomicoRealRepository.IncluirAsync(indicadorEconomico.Value);
            }

            return Result.Success("Indicador econômico incluido com sucesso");
        }

        private async ValueTask<Result> Atualizar(UpsertIndicadorEconomicoRealRequest request)
        {
            var indicadorEconomico = await _indicadorEconomicoRealRepository.ObterAsync(request.Id);

            if (indicadorEconomico == null)
            {
                return Result.Failure("Indicador econômico real não foi encontrado");
            }

            foreach (var periodoReal in request.PeriodosReais)
            {
                var result = indicadorEconomico.Atualizar(request.Indicador, periodoReal.Periodo, periodoReal.Real);

                if (result.IsFailure)
                {
                    return Result.Failure(result.Error);
                }

                await _indicadorEconomicoRealRepository.AtualizarAsync(indicadorEconomico);
            }

            return Result.Success("Indicador econômico atualizado com sucesso");
        }
    }
}
