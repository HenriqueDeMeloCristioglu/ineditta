using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.Localizacoes.Enumerations
{
    public class EstadoEnumeration : EnumValueObject<EstadoEnumeration>
    {
        public static readonly EstadoEnumeration Acre = new EstadoEnumeration("Acre");
        public static readonly EstadoEnumeration Alagoas = new EstadoEnumeration("Alagoas");
        public static readonly EstadoEnumeration Amapá = new EstadoEnumeration("Amapá");
        public static readonly EstadoEnumeration Amazonas = new EstadoEnumeration("Amazonas");
        public static readonly EstadoEnumeration Bahia = new EstadoEnumeration("Bahia");
        public static readonly EstadoEnumeration Ceará = new EstadoEnumeration("Ceará");
        public static readonly EstadoEnumeration DistritoFederal = new EstadoEnumeration("Distrito Federal");
        public static readonly EstadoEnumeration EspíritoSanto = new EstadoEnumeration("Espírito Santo");
        public static readonly EstadoEnumeration Goiás = new EstadoEnumeration("Goiás");
        public static readonly EstadoEnumeration Maranhão = new EstadoEnumeration("Maranhão");
        public static readonly EstadoEnumeration MatoGrosso = new EstadoEnumeration("Mato Grosso");
        public static readonly EstadoEnumeration MatoGrossoDoSul = new EstadoEnumeration("Mato Grosso do Sul");
        public static readonly EstadoEnumeration MinasGerais = new EstadoEnumeration("Minas Gerais");
        public static readonly EstadoEnumeration Pará = new EstadoEnumeration("Pará");
        public static readonly EstadoEnumeration Paraíba = new EstadoEnumeration("Paraíba");
        public static readonly EstadoEnumeration Paraná = new EstadoEnumeration("Paraná");
        public static readonly EstadoEnumeration Pernambuco = new EstadoEnumeration("Pernambuco");
        public static readonly EstadoEnumeration Piauí = new EstadoEnumeration("Piauí");
        public static readonly EstadoEnumeration RioDeJaneiro = new EstadoEnumeration("Rio de Janeiro");
        public static readonly EstadoEnumeration RioGrandeDoNorte = new EstadoEnumeration("Rio Grande do Norte");
        public static readonly EstadoEnumeration RioGrandeDoSul = new EstadoEnumeration("Rio Grande do Sul");
        public static readonly EstadoEnumeration Rondônia = new EstadoEnumeration("Rondônia");
        public static readonly EstadoEnumeration Roraima = new EstadoEnumeration("Roraima");
        public static readonly EstadoEnumeration SantaCatarina = new EstadoEnumeration("Santa Catarina");
        public static readonly EstadoEnumeration SãoPaulo = new EstadoEnumeration("São Paulo");
        public static readonly EstadoEnumeration Sergipe = new EstadoEnumeration("Sergipe");
        public static readonly EstadoEnumeration Tocantins = new EstadoEnumeration("Tocantins");


        private EstadoEnumeration(string id) : base(id)
        {
        }
    }
}
