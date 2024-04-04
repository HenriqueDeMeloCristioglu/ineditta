using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Atualizar
{
    public class AtualizarClienteUnidadeSindicatoPatronalRequest : IRequest<Result>
    {
        public IEnumerable<int>? ClienteUnidadeId { get; set; }
        public int SindicatoPatronalId { get; set; }
    }
}
