using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Deletar
{
    public class DeletarClienteUnidadeSindicatoPatronalRequestValidator : AbstractValidator<DeletarClienteUnidadeSindicatoPatronalRequest>
    {
        public DeletarClienteUnidadeSindicatoPatronalRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Para deleção você deve fornecer o id da entidade")
                .GreaterThan(0)
                .WithMessage("O id deve ser maior que 0");
        }
    }
}
