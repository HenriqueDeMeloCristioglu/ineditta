using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString;

namespace Ineditta.Application.BasesTerritoriaisLaborais.Entities
{
    public enum DataNegociacao
    {
        [Description("JAN")]
        Janeiro = 1,

        [Description("FEV")]
        Fevereiro = 2,

        [Description("MAR")]
        Marco = 3,

        [Description("ABR")]
        Abril = 4,

        [Description("MAI")]
        Maio = 5,

        [Description("JUN")]
        Junho = 6,

        [Description("JUL")]
        Julho = 7,

        [Description("AGO")]
        Agosto = 8,

        [Description("SET")]
        Setembro = 9,

        [Description("OUT")]
        Outubro = 10,

        [Description("NOV")]
        Novembro = 11,

        [Description("DEZ")]
        Dezembro = 12
    }
}
