using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Dtos
{
    public class EmailAssembleiaPatronalDto
    {
        public DateTime? DataHora { get; set; }
        public string? SindicatosLaborais { get; set; }
        public string? SindicatosPatronais { get; set; }
        public string? AtividadesEconomicas { get; set; }
        public string? DataBase { get; set; }
        public string? NomeDocumento { get; set; }
        public string? FaseNegociacao { get; set; }
        public string? Abrangencia { get; set; }
        public string? TipoAssembleiaPatronal { get; set; }
        public string? EnderecoAssembleiaPatronal { get; set; }
        public string? ComentarioAssembleiaPatronal { get; set; }
        public string? ClientReport { get; set; }
    }
}
