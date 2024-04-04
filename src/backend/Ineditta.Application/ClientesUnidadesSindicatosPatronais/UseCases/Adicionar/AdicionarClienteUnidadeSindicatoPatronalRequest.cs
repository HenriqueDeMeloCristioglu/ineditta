using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Adicionar
{
    public class AdicionarClienteUnidadeSindicatoPatronalRequest : IRequest<Result>
    {
        public int ClienteUnidadeId { get; set; }
        public int SindicatoPatronalId { get; set; }
    }
}
