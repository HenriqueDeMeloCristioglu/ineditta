using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Entities;
using Ineditta.Application.CalendarioSindicais.Eventos.Repositories;

using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.ClientesUnidades.Repositories;
using Ineditta.Application.ClientesUnidades.UseCases;
using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

using MediatR;

using Org.BouncyCastle.Asn1.Ocsp;

namespace Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Upsert
{
    public class UpsertEventoRequestHandler : BaseCommandHandler, IRequestHandler<UpsertEventoRequest, Result>
    {
        private readonly IEventoRepository _eventoRepository;
        public UpsertEventoRequestHandler(IUnitOfWork unitOfWork, IEventoRepository eventoRepository) : base(unitOfWork)
        {
            _eventoRepository = eventoRepository;
        }

        public async Task<Result> Handle(UpsertEventoRequest request, CancellationToken cancellationToken)
        {
            return request.Id > 0 ?
                await AtualizarAsync(request, cancellationToken) :
                await IncluirAsync(request, cancellationToken);
        }
        private async Task<Result> AtualizarAsync(UpsertEventoRequest request, CancellationToken cancellationToken)
        {
            var evento = await _eventoRepository.ObterPorIdAsync(request.Id ?? 0);

            if (evento is null)
            {
                return Result.Failure("Evento não encontrada");
            }

            var tipoEvento = TipoEventoCalendarioSindical.ObterPorId(request.TipoEvento);
            if (tipoEvento.IsFailure) return Result.Failure("Tipo de evento não reconhecido");

            var result = evento.Atualizar(
                request.ChaveReferenciaId,
                tipoEvento.Value,
                request.SubtipoEvento,
                request.Origem,
                request.DataReferencia,
                request.NotificarAntes,
                request.Ativo ?? true
            );

            if (result.IsFailure)
            {
                return result;
            }

            await _eventoRepository.AtualizarAsync(evento);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<Result> IncluirAsync(UpsertEventoRequest request, CancellationToken cancellationToken)
        {
            var tipoEvento = TipoEventoCalendarioSindical.ObterPorId(request.TipoEvento);
            if (tipoEvento.IsFailure) return Result.Failure("Tipo de evento não reconhecido");

            var evento = CalendarioSindical.Criar(
                request.ChaveReferenciaId,
                tipoEvento.Value,
                request.SubtipoEvento,
                request.Origem,
                request.DataReferencia,
                request.NotificarAntes
            );

            if (evento.IsFailure)
            {
                return evento;
            }

            await _eventoRepository.IncluirAsync(evento.Value);

            _ = await CommitAsync(cancellationToken);

            return Result.Success();
        }
    }
}
