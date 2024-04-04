using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Atualizar
{
    public class AtualizarClienteUnidadeSindicatoPatronalRequestValidator : AbstractValidator<AtualizarClienteUnidadeSindicatoPatronalRequest>
    {
        public AtualizarClienteUnidadeSindicatoPatronalRequestValidator()
        {
            When(x => x.ClienteUnidadeId is not null && x.ClienteUnidadeId.Any(), () =>
            {
                RuleFor(x => x.ClienteUnidadeId)
                    .Must(cut => cut is not null && !cut.Any(v => v <= 0))
                    .WithMessage("O id do cliente unidade deve ser maior que 0");
            });

            RuleFor(x => x.SindicatoPatronalId)
                .GreaterThan(0)
                .WithMessage("O id do sindicato patronal deve ser maior que 0");
        }
    }
}
