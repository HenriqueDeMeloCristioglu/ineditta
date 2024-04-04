using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.Application.ClientesMatriz.UseCases.InativarAtivarToggle
{
    public class InativarAtivarClienteMatrizRequest : IRequest<Result>
    {
        public int Id { get; set; }
    }
}
