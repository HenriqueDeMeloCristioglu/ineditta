using System.ComponentModel;

using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

namespace Ineditta.Application.CalendarioSindicais.Eventos.Entities
{
    [BindAsString]
    public enum OrigemEvento
    {
        [Description("Ineditta")]
        Ineditta = 1,

        [Description("Cliente")]
        Cliente,
    }
}