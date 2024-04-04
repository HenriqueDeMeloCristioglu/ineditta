using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Deletar
{
    public class DeletarClienteUnidadeSindicatoPatronalRequest : IRequest<Result>
    {
        public long Id { get; set; }
    }
}
