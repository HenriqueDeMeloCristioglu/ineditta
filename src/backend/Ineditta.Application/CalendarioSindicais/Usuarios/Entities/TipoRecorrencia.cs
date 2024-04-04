using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.CalendarioSindicais.Usuarios.Entities
{
    public enum TipoRecorrencia
    {
        [Description("Não repetir")]
        NaoRepetir = 1,

        [Description("Semanalmente")]
        Semanal,

        [Description("A cada 15 dias")]
        Quinzenal,

        [Description("A cada 2 meses")]
        Bimestral,

        [Description("A cada 3 meses")]
        Trimestral,

        [Description("A cada 6 meses")]
        Semestral,

        [Description("Anualmente")]
        Anual,

        [Description("Mensalmente")]
        Mensal
    }
}
