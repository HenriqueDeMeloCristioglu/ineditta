using System.ComponentModel;

using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

namespace Ineditta.Application.Clausulas.Gerais.Entities
{
    [BindAsString]
    public enum Aprovacao
    {
        [Description("sim")]
        Sim,
        [Description("não")]
        Nao
    }
}
