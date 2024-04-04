using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.ClientesMatriz.Errors
{
    public static class ClienteMatrizError
    {
        public static string NullOrEmptyArgumentError(string nomeCampo)
        {
            return "Você deve fornecer um valor para o(a)" + nomeCampo;
        }
    }
}
