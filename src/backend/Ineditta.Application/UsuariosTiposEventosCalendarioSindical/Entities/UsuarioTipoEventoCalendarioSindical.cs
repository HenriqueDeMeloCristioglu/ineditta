using CSharpFunctionalExtensions;

namespace Ineditta.Application.UsuariosTiposEventosCalendarioSindical.Entities
{
    public class UsuarioTipoEventoCalendarioSindical : Entity
    {
        private UsuarioTipoEventoCalendarioSindical(int usuarioId, int tipoEventoId, int? subtipoEventoId, bool notificarEmail, bool notificarWhatsapp, TimeSpan notificarAntes)
        {
            UsuarioId = usuarioId;
            TipoEventoId = tipoEventoId;
            SubtipoEventoId = subtipoEventoId;
            NotificarEmail = notificarEmail;
            NotificarWhatsapp = notificarWhatsapp;
            NotificarAntes = notificarAntes;
        }

        protected UsuarioTipoEventoCalendarioSindical() { }

        public int UsuarioId { get; private set; }
        public int TipoEventoId { get; private set; }
        public int? SubtipoEventoId { get; private set; }
        public bool NotificarEmail { get; private set; }
        public bool NotificarWhatsapp { get; private set; }
        public TimeSpan NotificarAntes { get; private set; }

        public static Result<UsuarioTipoEventoCalendarioSindical> Criar(int usuarioId, int tipoEventoId, int? subtipoEventoId, bool notificarEmail, bool notificarWhatsapp, TimeSpan? notificarAntes)
        {
            if (usuarioId <= 0) return Result.Failure<UsuarioTipoEventoCalendarioSindical>("O id do usuário deve ser maior ou igual que 0");
            if (tipoEventoId <= 0) return Result.Failure<UsuarioTipoEventoCalendarioSindical>("O tipo do evento deve ser maior ou igual que 0");
            if (subtipoEventoId is not null && subtipoEventoId <= 0) return Result.Failure<UsuarioTipoEventoCalendarioSindical>("O subtipo do evento deve ser maior ou igual que 0");

            var usuarioTipoEventoCalendarioSindical = new UsuarioTipoEventoCalendarioSindical(
                usuarioId,
                tipoEventoId,
                subtipoEventoId,
                notificarEmail,
                notificarWhatsapp,
                notificarAntes ?? TimeSpan.FromDays(5)
            );

            return Result.Success(usuarioTipoEventoCalendarioSindical);
        }

        public Result Atualizar(bool notificarEmail, bool notificarWhatsapp, TimeSpan? notificarAntes = null)
        {
            NotificarEmail = notificarEmail;
            NotificarWhatsapp = notificarWhatsapp;
            NotificarAntes = notificarAntes ?? TimeSpan.FromDays(5);

            return Result.Success();
        }
    }
}
