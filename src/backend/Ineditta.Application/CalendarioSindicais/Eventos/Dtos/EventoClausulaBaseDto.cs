using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public sealed class EventoClausulaBaseDto
    {
        public long? ChaveReferencia { get; set; }
        public DateOnly? DataReferencia { get; set; }
        public DateOnly? VigenciaFinal { get; set; }
        public string? Frequencia { get; set; }
        public int? SubtipoEvento { get; set; }
    }
}
