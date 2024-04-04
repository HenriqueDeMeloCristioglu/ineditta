using System.ComponentModel;

namespace Ineditta.Application.Sindicatos.Laborais.Entities
{
    public enum Grau
    {
        [Description("Sindicato")]
        Sindicato = 0,

        [Description("Federação")]
        Federacao = 1,

        [Description("Confederação")]
        Confederacao = 2
    }
}
