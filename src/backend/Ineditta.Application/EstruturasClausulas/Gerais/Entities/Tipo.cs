using System.ComponentModel;

namespace Ineditta.Application.EstruturasClausulas.Gerais.Entities
{
    public enum Tipo
    {
        [Description("T")]
        T = 1,
        [Description("N")]
        N = 2,
        [Description("P")]
        P = 3,
        [Description("NA")]
        NA = 4
    }
}
