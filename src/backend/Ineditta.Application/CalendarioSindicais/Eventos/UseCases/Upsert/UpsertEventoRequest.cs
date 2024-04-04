using CSharpFunctionalExtensions;

using Ineditta.Application.CalendarioSindicais.Eventos.Entities;

using MediatR;

namespace Ineditta.Application.CalendarioSindicais.Eventos.UseCases.Upsert
{
    public class UpsertEventoRequest : IRequest<Result>
    {
        public int? Id { get; set; }
        public long ChaveReferenciaId { get; set; }
        public int TipoEvento { get; set; }
        public int? SubtipoEvento { get; set; }
        public OrigemEvento Origem { get; set; }
        public TimeSpan? NotificarAntes { get; set; }
        public DateTime DataReferencia { get; set; }
        public bool? Ativo { get; set; }
    }
}
