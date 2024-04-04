using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class EmailVencimentoMandatoLaboralDto
    {
        public DateOnly? DataVencimento { get; set; }
        public string? SindicatoLaboral { get; set; }
        public string? Abrangencia { get; set; }
        public IEnumerable<Dirigente>? Dirigente { get; set; }
    }
}
