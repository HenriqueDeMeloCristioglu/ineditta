using CSharpFunctionalExtensions;

using Ineditta.Application.TiposEventosCalendarioSindical.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Entities
{
    public class CalendarioSindical : Entity, IAuditable
    {
        private CalendarioSindical(long chaveReferenciaId, int tipoEvento, int? subtipoEvento, OrigemEvento origem, DateTime dataReferencia, TimeSpan? notificarAntes, bool ativo)
        {
            ChaveReferenciaId = chaveReferenciaId;
            TipoEvento = tipoEvento;
            SubtipoEvento = subtipoEvento;
            Origem = origem;
            DataReferencia = dataReferencia;
            NotificarAntes = notificarAntes;
            Ativo = ativo;
        }

        protected CalendarioSindical() { }

        public long ChaveReferenciaId { get; private set; }
        public int TipoEvento { get; private set; }
        public int? SubtipoEvento { get; set; }
        public OrigemEvento Origem { get; private set; }
        public DateTime DataReferencia { get; set; }
        public TimeSpan? NotificarAntes { get; private set; }
        public bool Ativo { get; set; }

        public static Result<CalendarioSindical> Criar(long chaveReferenciaId, TipoEventoCalendarioSindical tipoEvento, int? subtipoEvento, OrigemEvento origem, DateTime dataReferencia, TimeSpan? notificarAntes)
        {
            if (chaveReferenciaId <= 0) return Result.Failure<CalendarioSindical>("A chaveReferenciaId deve ser maior que 0");
            if (dataReferencia < DateTime.Now) return Result.Failure<CalendarioSindical>("Você não pode criar um evento para o passado.");
            if (subtipoEvento is not null && subtipoEvento <= 0) Result.Failure<CalendarioSindical>("O subtipo de evento deve ser maior que 0");
            if (tipoEvento is null) return Result.Failure<CalendarioSindical>("Você deve fornecer um tipo de evento");

            CalendarioSindical evento = new(chaveReferenciaId, tipoEvento.Id, subtipoEvento, origem, dataReferencia, notificarAntes, true);
            return Result.Success(evento);
        }

        public Result Atualizar(
            long chaveReferenciaId,
            TipoEventoCalendarioSindical tipoEvento,
            int? subtipoEvento,
            OrigemEvento origem,
            DateTime dataReferencia,
            TimeSpan? notificarAntes,
            bool ativo
            )
        {
            if (chaveReferenciaId <= 0) return Result.Failure<CalendarioSindical>("A chaveReferenciaId deve ser maior que 0");
            if (subtipoEvento is not null && subtipoEvento <= 0) Result.Failure<CalendarioSindical>("O subtipo de evento deve ser maior que 0");
            if (tipoEvento is null) return Result.Failure<CalendarioSindical>("Você deve fornecer um tipo de evento");

            ChaveReferenciaId = chaveReferenciaId;
            TipoEvento = tipoEvento.Id;
            SubtipoEvento = subtipoEvento;
            Origem = origem;
            NotificarAntes = notificarAntes;
            DataReferencia = dataReferencia;
            Ativo = ativo;

            return Result.Success();
        }
    }
}
