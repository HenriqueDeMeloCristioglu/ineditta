using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;

namespace Ineditta.API.ViewModels.EventosCalendario.ViewModels
{
    public class EventoUsuarioViewModel
    {
        public long? IdEvento { get; set; }
        public string? Titulo { get; set; }
        public DateTime? Data { get; set; }
        public TipoRecorrencia? RecorrenciaNumero { get; set; }
        public TipoRecorrencia? Recorrencia { get; set; }
        public DateTime? ValidadeRecorrencia { get; set; }
        public string? Local { get; set; }
        public string? Comentario { get; set; }
        public bool? Visivel { get; set; }
    }
}
