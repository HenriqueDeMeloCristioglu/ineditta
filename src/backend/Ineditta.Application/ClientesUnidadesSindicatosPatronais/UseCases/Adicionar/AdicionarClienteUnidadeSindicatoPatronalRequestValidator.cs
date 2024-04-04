using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.ClientesUnidadesSindicatosPatronais.UseCases.Adicionar
{
    public class AdicionarClienteUnidadeSindicatoPatronalRequestValidator : AbstractValidator<AdicionarClienteUnidadeSindicatoPatronalRequest>
    {
        public AdicionarClienteUnidadeSindicatoPatronalRequestValidator()
        {
            RuleFor(x => x.ClienteUnidadeId)
                .NotEmpty()
                .WithMessage("Você deve fornecer o cliente unidade id")
                .GreaterThan(0)
                .WithMessage("O id do cliente unidade deve ser maior que o");

            RuleFor(x => x.SindicatoPatronalId)
                .NotEmpty()
                .WithMessage("Você deve fornecer um id para o sindicato patronal")
                .GreaterThan(0)
                .WithMessage("O id do sindicato patronal deve ser maior que 0");
        }
    }
}
