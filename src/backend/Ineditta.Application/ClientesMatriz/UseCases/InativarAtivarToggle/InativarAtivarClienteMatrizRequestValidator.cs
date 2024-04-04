using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.ClientesMatriz.UseCases.InativarAtivarToggle
{
    public sealed class InativarAtivarClienteMatrizRequestValidator : AbstractValidator<InativarAtivarClienteMatrizRequest>
    {
        public InativarAtivarClienteMatrizRequestValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty()
                .WithMessage("Você deve fornecer o id do cliente matriz para inativar/ativar")
                .GreaterThan(0)
                .WithMessage("O id do cliente matriz deve ser maior que 0");
        }
    }
}
