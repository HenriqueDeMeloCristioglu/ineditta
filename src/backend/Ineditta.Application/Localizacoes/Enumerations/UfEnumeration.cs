using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.Localizacoes.Enumerations
{
    public class UfEnumeration : EnumValueObject<UfEnumeration>
    {
        public static readonly UfEnumeration AC = new UfEnumeration("AC");
        public static readonly UfEnumeration AL = new UfEnumeration("AL");
        public static readonly UfEnumeration AP = new UfEnumeration("AP");
        public static readonly UfEnumeration AM = new UfEnumeration("AM");
        public static readonly UfEnumeration BA = new UfEnumeration("BA");
        public static readonly UfEnumeration CE = new UfEnumeration("CE");
        public static readonly UfEnumeration DF = new UfEnumeration("DF");
        public static readonly UfEnumeration ES = new UfEnumeration("ES");
        public static readonly UfEnumeration GO = new UfEnumeration("GO");
        public static readonly UfEnumeration MA = new UfEnumeration("MA");
        public static readonly UfEnumeration MT = new UfEnumeration("MT");
        public static readonly UfEnumeration MS = new UfEnumeration("MS");
        public static readonly UfEnumeration MG = new UfEnumeration("MG");
        public static readonly UfEnumeration PA = new UfEnumeration("PA");
        public static readonly UfEnumeration PB = new UfEnumeration("PB");
        public static readonly UfEnumeration PR = new UfEnumeration("PR");
        public static readonly UfEnumeration PE = new UfEnumeration("PE");
        public static readonly UfEnumeration PI = new UfEnumeration("PI");
        public static readonly UfEnumeration RJ = new UfEnumeration("RJ");
        public static readonly UfEnumeration RN = new UfEnumeration("RN");
        public static readonly UfEnumeration RS = new UfEnumeration("RS");
        public static readonly UfEnumeration RO = new UfEnumeration("RO");
        public static readonly UfEnumeration RR = new UfEnumeration("RR");
        public static readonly UfEnumeration SC = new UfEnumeration("SC");
        public static readonly UfEnumeration SP = new UfEnumeration("SP");
        public static readonly UfEnumeration SE = new UfEnumeration("SE");
        public static readonly UfEnumeration TO = new UfEnumeration("TO");
        private UfEnumeration(string id) : base(id)
        {
        }
    }
}
