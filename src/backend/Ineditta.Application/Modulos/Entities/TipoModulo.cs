using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.Modulos.Entities
{
    public enum TipoModulo
    {
        [Description("Comercial")]
        Comercial = 1,

        [Description("SISAP")]
        Sisap = 2,

        [Description("Helpdesk")]
        Helpdesk = 3,
    }
}
