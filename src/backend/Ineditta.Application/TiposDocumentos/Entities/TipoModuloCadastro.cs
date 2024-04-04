using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.TiposDocumentos.Entities
{
    public enum TipoModuloCadastro
    {
        [Description("Geral")]
        Geral = 1,

        [Description("Processado")]
        Processado = 2,
    }
}
