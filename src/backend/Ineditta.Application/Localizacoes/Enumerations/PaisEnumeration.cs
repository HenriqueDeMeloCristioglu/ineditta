using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.Localizacoes.Enumerations
{
    public class PaisEnumeration : EnumValueObject<PaisEnumeration>
    {
        public static readonly PaisEnumeration Brasil = new PaisEnumeration("Brasil");
        private PaisEnumeration(string id) : base(id)
        {
        }
    }
}
