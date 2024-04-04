using System.ComponentModel;

using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

namespace Ineditta.Application.DocumentosLocalizados.Entities
{
    [BindAsString]
    public enum Situacao
    {
        [Description("não aprovado")]
        NaoAprovado,
        [Description("aprovado")]
        Aprovado
    }
}
