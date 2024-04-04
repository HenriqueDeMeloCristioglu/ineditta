using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class EmailEventoClausulaDto
    {
        public string? NomeCampo { get; set; }
        public string? NomeInfoAdicional { get; set; }
        public string? NomeDocumento { get; set; }
        public DateOnly? DataVencimento { get; set; }
        public string? Abrangencias { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? SindicatosPatronais { get; set; }
        public string? SindicatosLaborais { get; set; }
    }
}
