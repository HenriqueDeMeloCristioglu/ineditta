using System.ComponentModel;

using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

namespace Ineditta.Application.Documentos.Sindicais.Entities
{
    [BindAsString]
    public enum TipoModulo
    {
        [Description("SISAP")]
        SISAP = 1,
        [Description("Comercial")]
        Comercial = 2
    }
}
