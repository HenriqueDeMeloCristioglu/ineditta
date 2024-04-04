﻿using System.Globalization;

using CSharpFunctionalExtensions;

using Ineditta.Application.AcompanhamentosCcts.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Upsert;
using Ineditta.Application.CctsFases.Entities;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;

using MediatR;

namespace Ineditta.Application.Acompanhamentos.Ccts.Services.AcompanhamentosCctsEventos
{
    public class AcompanhamentoCctEventoAssembleiaPatronalService
    {
        private readonly IMediator _mediator;

        public AcompanhamentoCctEventoAssembleiaPatronalService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Criar(long acompanhamentoCctId, IEnumerable<string> respostas, long faseCctId, CancellationToken cancellationToken)
        {
            if (respostas.Any() && respostas.Count() >= 11 &&
                (faseCctId == FasesCct.IndiceNegociacaoNaoIniciada || faseCctId == FasesCct.IndiceAssembleiaPatronal || faseCctId == FasesCct.IndiceEmNegociacao) && !string.IsNullOrEmpty(respostas.ToList()[Script.IndiceDataAssembleiaPatronal]))
            {
                string dataRequest = respostas.ToList()[Script.IndiceDataAssembleiaPatronal];
                string horaRequest = respostas.ToList()[Script.IndiceHoraAssembleiaPatronal];

                if (DateTime.TryParse(dataRequest, CultureInfo.InvariantCulture, out DateTime data))
                {
                    if (!string.IsNullOrEmpty(horaRequest) && TimeSpan.TryParse(horaRequest, CultureInfo.InvariantCulture, out TimeSpan hora))
                    {
                        data = data.Add(hora);
                    }
                    else
                    {
                        return Result.Failure("Hora da Assembléia patronal inválida");
                    }

                    var eventoRequest = new UpsertEventoRequest
                    {
                        ChaveReferenciaId = acompanhamentoCctId,
                        TipoEvento = TipoEventoCalendarioSindical.AssembleiaPatronal.Id,
                        Origem = OrigemEvento.Ineditta,
                        DataReferencia = data,
                        NotificarAntes = TimeSpan.FromDays(5)
                    };

                    await _mediator.Send(eventoRequest, cancellationToken);
                }
                else
                {
                    return Result.Failure("Data da Assembléia patronal inválida");
                }
            }

            return Result.Success();
        }
    }
}