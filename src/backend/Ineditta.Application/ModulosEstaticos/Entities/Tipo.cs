using System.ComponentModel;

namespace Ineditta.Application.ModulosAcompanhamentoCct.Entities
{
    public enum Tipo
    {
        [Description("Comercial")]
        Comercial = 1,
        [Description("SISAP")]
        Sisap = 2,
        [Description("Helpdesk")]
        Helpdesk = 3
    }
}
