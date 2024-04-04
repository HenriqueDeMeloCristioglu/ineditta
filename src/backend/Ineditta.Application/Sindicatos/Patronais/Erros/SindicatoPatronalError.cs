using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ineditta.Application.Sindicatos.Patronais.Erros
{
    public static class SindicatoPatronalError
    {
        public static string CampoVazio(string campo)
        {
            return "Campo " + campo + "vazio";
        }

        public static string CampoInvalido(string campo)
        {
            return "Campo " + campo + "inválido";
        }
    }
}
