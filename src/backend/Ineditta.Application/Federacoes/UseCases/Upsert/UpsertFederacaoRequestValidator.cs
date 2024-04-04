using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;

namespace Ineditta.Application.Federacoes.UseCases.Upsert
{
    public class UpsertFederacaoRequestValidator : AbstractValidator<UpsertFederacaoRequest>
    {
        public UpsertFederacaoRequestValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("Informe o código");

            RuleFor(p => p.Sigla)
                .NotEmpty()
                .WithMessage("Informe a sigla")
                .MaximumLength(100)
                .WithMessage("Sigla deve ter no máximo 100 caracteres");

            RuleFor(p => p.CNPJ)
                .NotEmpty()
                .WithMessage("Informe o CNPJ");

            RuleFor(p => p.Telefone)
                .NotEmpty()
                .WithMessage("Informe um telefone")
                .MaximumLength(15)
                .WithMessage("Telefone deve ter no máximo 15 caracteres");

            RuleFor(p => p.AreaGeoeconomica)
                .NotEmpty()
                .WithMessage("Informe a Área GeoEconômica")
                .MaximumLength(250)
                .WithMessage("Área GeoEconômica deve ter no máximo 250 caracteres");

            RuleFor(p => p.Grupo)
                .NotEmpty()
                .WithMessage("Informe o grupo")
                .MaximumLength(200)
                .WithMessage("Grupo deve ter no máximo 200 caracteres");

            RuleFor(p => p.Grau)
                .NotEmpty()
                .WithMessage("Informe o grau")
                .MaximumLength(200)
                .WithMessage("Grau deve ter no máximo 200 caracteres");
        }
    }
}
