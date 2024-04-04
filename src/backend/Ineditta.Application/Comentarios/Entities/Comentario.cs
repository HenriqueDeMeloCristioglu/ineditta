using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.Comentarios.Entities
{
    public class Comentario : Entity, IAuditable
    {
        public TipoComentario Tipo { get; private set; }
        public string Valor { get; private set; } = null!;
        public TipoNotificacao TipoNotificacao { get; private set; }
        public int ReferenciaId { get; private set; }
        public DateOnly? DataValidade { get; private set; }
        public TipoUsuarioDestino TipoUsuarioDestino { get; private set; }
        public int UsuarioDestionoId { get; private set; }
        public long EtiquetaId { get; private set; }
        public bool Visivel { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Comentario() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private Comentario(TipoComentario tipo, string valor, TipoNotificacao tipoNotificacao, int referenciaId, DateOnly? dataValidade, TipoUsuarioDestino tipoUsuarioDestino, int usuarioDestionoId, long etiquetaId, bool visivel)
        {
            Tipo = tipo;
            Valor = valor;
            TipoNotificacao = tipoNotificacao;
            ReferenciaId = referenciaId;
            DataValidade = dataValidade;
            TipoUsuarioDestino = tipoUsuarioDestino;
            UsuarioDestionoId = usuarioDestionoId;
            EtiquetaId = etiquetaId;
            Visivel = visivel;
        }

        internal static Result<Comentario> Criar(TipoComentario tipo, string valor, TipoNotificacao tipoNotificacao, int referenciaId, DateOnly? dataValidade, TipoUsuarioDestino tipoUsuarioDestino, int usuarioDestionoId, long etiquetaId, bool visivel)
        {
            if (tipo < 0) return Result.Failure<Comentario>("O Tipo de Comentário não pode ser nulo");
            if (valor is null) return Result.Failure<Comentario>("O Valor do comentário não pode ser nulo");
            if (tipoNotificacao < 0) return Result.Failure<Comentario>("O Tipo de Notificação não pode ser nulo");
            if (tipoNotificacao == TipoNotificacao.Temporaria && dataValidade is null)
            {
                return Result.Failure<Comentario>("O campo Data Validade não pode ser nulo para comentário temporário");
            }
            if (referenciaId < 0) return Result.Failure<Comentario>("A Referência não pode ser nula");
            if (tipoUsuarioDestino < 0) return Result.Failure<Comentario>("O Tipo de Usuario Destino não pode ser nulo");
            if (usuarioDestionoId < 0) return Result.Failure<Comentario>("O Id do Usuario Destino não pode ser nulo");
            if (etiquetaId < 0) return Result.Failure<Comentario>("O Id da Etiqueta não pode ser nulo");

            var comentario = new Comentario(
                tipo,
                valor,
                tipoNotificacao,
                referenciaId,
                dataValidade,
                tipoUsuarioDestino,
                usuarioDestionoId,
                etiquetaId,
                visivel
            );

            return Result.Success(comentario);
        }

        internal Result Atualizar(TipoComentario tipo, string valor, TipoNotificacao tipoNotificacao, int referenciaId, DateOnly? dataValidade, TipoUsuarioDestino tipoUsuarioDestino, int usuarioDestionoId, long etiquetaId, bool visivel)
        {
            if (tipo < 0) return Result.Failure<Comentario>("O Tipo de Comentário não pode ser nulo");
            if (valor is null) return Result.Failure<Comentario>("O Valor do comentário não pode ser nulo");
            if (tipoNotificacao < 0) return Result.Failure<Comentario>("O Tipo de Notificação não pode ser nulo");
            if (referenciaId < 0) return Result.Failure<Comentario>("A Referência não pode ser nula");
            if (tipoUsuarioDestino < 0) return Result.Failure<Comentario>("O Tipo de Usuario Destino não pode ser nulo");
            if (usuarioDestionoId < 0) return Result.Failure<Comentario>("O Id do Usuario Destino não pode ser nulo");
            if (etiquetaId < 0) return Result.Failure<Comentario>("O Id da Etiqueta não pode ser nulo");

            Tipo = tipo;
            Valor = valor;
            TipoNotificacao = tipoNotificacao;
            ReferenciaId = referenciaId;
            DataValidade = dataValidade;
            TipoUsuarioDestino = tipoUsuarioDestino;
            UsuarioDestionoId = usuarioDestionoId;
            EtiquetaId = etiquetaId;
            Visivel = visivel;

            return Result.Success();
        }
    }
}
