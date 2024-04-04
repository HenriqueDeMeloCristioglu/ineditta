using System.ComponentModel;

using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

namespace Ineditta.Application.Usuarios.Entities
{
    [BindAsString]
    public enum Nivel
    {
        [Description("Ineditta")]
        Ineditta,
        [Description("Unidade")]
        Unidade,
        [Description("Matriz")]
        Matriz,
        [Description("Grupo Econômico")]
        GrupoEconomico
    }
}