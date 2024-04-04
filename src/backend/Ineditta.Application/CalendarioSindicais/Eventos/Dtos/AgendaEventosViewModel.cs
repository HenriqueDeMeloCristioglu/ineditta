using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;
using Ineditta.Application.Usuarios.Entities;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class AgendaEventosViewModel
    {
        public long? Id { get; set; }
        public long? CalendarioSindicalUsuarioId { get; set; }
        public DateTime? DataVencimento { get; set; }
        public string? Titulo { get; set; }
        public TipoRecorrencia? Recorrencia { get; set; }
        public TimeSpan? NotificarAntes { get; set; }
        public string? Local { get; set; }
        public string? Comentarios { get; set; }
        public bool Visivel { get; set; }
        public DateTime? DataInclusao { get; set; }
        public DateTime? ValidadeRecorrencia { get; set; }
        public long? UsuarioCriadorId { get; set; }
        public string? UsuarioCriadorEmail { get; set; }
        public bool UsuarioNotificarEmail { get; set; }
    }
}
