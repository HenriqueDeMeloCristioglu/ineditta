using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class EmailEventoTrintidioDto
    {
        public string? Abrangencia { get; set; }
        public string? NomeDocumento { get; set; } = null!;
        public string? AtividadesEconomicas { get; set; } = null!;
        public DateOnly? VigenciaInicio { get; set; }
        public DateOnly? VigenciaFinal { get; set; }
        public string? SindicatoLaboral { get; set; }
        public string? SindicatoPatronal { get; set; }
    }
}
