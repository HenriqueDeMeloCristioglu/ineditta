using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.Localizacoes.Enumerations
{
    public class RegiaoEnumeration : EnumValueObject<RegiaoEnumeration>
    {
        public static readonly RegiaoEnumeration Norte = new RegiaoEnumeration("Norte");
        public static readonly RegiaoEnumeration Nordeste = new RegiaoEnumeration("Nordeste");
        public static readonly RegiaoEnumeration CentroOeste  = new RegiaoEnumeration("Centro-Oeste");
        public static readonly RegiaoEnumeration Sul = new RegiaoEnumeration("Sul");
        public static readonly RegiaoEnumeration Sudeste  = new RegiaoEnumeration("Sudeste");

        private RegiaoEnumeration(string id) : base(id)
        {
        }
    }
}
