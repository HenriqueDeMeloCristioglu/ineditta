using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.DirigentesPatronais.UseCases
{
    public class UpsertDirigentePatronalRequestValidator : AbstractValidator<UpsertDirigentePatronalRequest>
    {
        public UpsertDirigentePatronalRequestValidator()
        {
            When(p => p.Id != null, () =>
            {
                RuleFor(p => p.Id)
                    .GreaterThan(0)
                    .WithMessage("O id do dirigente deve ser maior que 0");
            });

            RuleFor(p => p.Nome)
                .NotEmpty()
                .WithMessage("Você precisa fornecer um nome para o dirigente");

            RuleFor(p => p.Funcao)
                .NotEmpty()
                .WithMessage("Você precisa fornecer uma função para o dirigente");

            RuleFor(p => p.DataInicioMandato)
                .NotEmpty()
                .WithMessage("Forneça a data de início do mandato");

            RuleFor(p => p.DataFimMandato)
                .NotEmpty()
                .WithMessage("Forneça a data fim do mandato");

            RuleFor(p => p.DataFimMandato)
                .GreaterThan(p => p.DataInicioMandato)
                .WithMessage("A termino do mandato deve ocorrer depois do início");

            RuleFor(p => p.SindicatoPatronalId)
                .GreaterThan(0)
                .WithMessage("O id do sindicato patronal deve ser maior que 0");

            When(p => p.EstabelecimentoId != null, () =>
            {
                RuleFor(p => p.EstabelecimentoId)
                    .GreaterThan(0)
                    .WithMessage("O id do estabelecimento deve ser maior que 0");
            });
        }
    }
}
