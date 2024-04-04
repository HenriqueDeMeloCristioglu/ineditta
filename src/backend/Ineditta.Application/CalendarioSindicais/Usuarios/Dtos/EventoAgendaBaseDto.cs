using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ineditta.Application.CalendarioSindicais.Usuarios.Entities;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.Dtos
{
    public class EventoAgendaBaseDto
    {
        public int? ChaveReferencia { get; set; }
        public DateTime? DataReferencia { get; set; }
        public DateTime? ValidadeRecorrencia { get; set; }
        public TipoRecorrencia Frequencia { get; set; }
        public TimeSpan? NotificarAntes { get; set; }
    }
}
