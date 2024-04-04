using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.ClientesMatriz.Entities
{
    public enum TipoProcessamento
    {
        [Description("assinado ou registrado")]
        AssinadoOuRegistrado = 1,

        [Description("sem assinatura")]
        SemAssinatura = 2,

        [Description("somente registrado")]
        SomenteRegistrado = 3
    }
}
