using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class EmailAgendaEventosDto
    {
        public string? Titulo { get; set; }
        public DateTime? DataVencimento { get; set; }
        public string? Local { get; set; }
        public string? Recorrencia { get; set; }
        public DateTime? ValidadeRecorrencia { get; set; }
        public string? Visivel { get; set; }
        public string? Comentarios { get; set; }
    }
}
