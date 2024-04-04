using CSharpFunctionalExtensions;

using Ineditta.Application.Comentarios.Entities;

using MediatR;

namespace Ineditta.Application.Comentarios.UseCases.Upsert
{
    public class UpsertComentarioRequest : IRequest<Result>
    {
        public long Id { get; set; }
        public TipoComentario Tipo { get; set; }
        public string Valor { get; set; } = null!;
        public TipoNotificacao TipoNotificacao { get; set; }
        public int ReferenciaId { get; set; }
        public DateOnly? DataValidade { get; set; }
        public TipoUsuarioDestino TipoUsuarioDestino { get; set; }
        public int UsuarioDestionoId { get; set; }
        public long EtiquetaId { get; set; }
        public bool? Visivel { get; set; }
    }
}
