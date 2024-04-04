using FluentValidation;

using Ineditta.Application.Sindicatos.Base.ValueObjects;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.BuildingBlocks.Core.Validators;

namespace Ineditta.Application.Sindicatos.Laborais.UseCases.Upsert
{
    public class UpsertSindicatoLaboralValidator : AbstractValidator<UpsertSindicatoLaboralRequest>
    {
        public UpsertSindicatoLaboralValidator()
        {
            RuleFor(p => p.Sigla)
                .NotEmpty()
                .WithMessage("Informe a Sigla");

            RuleFor(p => p.Cnpj!)
                .NotEmpty()
                .WithMessage("Informe o Cnpj")
                .MustBeValueObject(CNPJ.Criar);

            RuleFor(p => p.RazaoSocial)
                .NotEmpty()
                .WithMessage("Informe a Razão Social");

            RuleFor(p => p.Denominacao)
                .NotEmpty()
                .WithMessage("Informe a Denominação");

            RuleFor(p => p.CodigoSindical!)
                .NotEmpty()
                .WithMessage("Informe o CodigoSindical")
                .MustBeValueObject(CodigoSindical.Criar);

            RuleFor(p => p.Logradouro)
                .NotEmpty()
                .WithMessage("Informe o Logradouro");

            RuleFor(p => p.Municipio)
                .NotEmpty()
                .WithMessage("Informe o Municipio");

            RuleFor(p => p.Uf)
                .NotEmpty()
                .WithMessage("Informe a Uf")
                .Length(2)
                .WithMessage("Uf deve ter 2 caracteres");

            RuleFor(p => p.Telefone1!)
                .NotEmpty()
                .WithMessage("Informe o Telefone 1")
                .MustBeValueObject(Telefone.Criar);

            When(p => !string.IsNullOrEmpty(p.Telefone2), () =>
            {
                RuleFor(p => p.Telefone2!)
               .MustBeValueObject(Telefone.Criar);
            });

            When(p => !string.IsNullOrEmpty(p.Telefone3), () => 
            {
                 RuleFor(p => p.Telefone3!)
                .MustBeValueObject(Telefone.Criar);
            });

            When(p => !string.IsNullOrEmpty(p.Ramal!), () =>
            {
                RuleFor(p => p.Ramal!)
               .MustBeValueObject(Ramal.Criar);
            });

            When(p => !string.IsNullOrEmpty(p.Email1!), () =>
            {
                RuleFor(p => p.Email1)
                .MaximumLength(200)
                .WithMessage("O email deve ter no máximo 200 caracteres")
                .EmailAddress()
                .WithMessage("Email inválido");
            });

            When(p => !string.IsNullOrEmpty(p.Email2!), () =>
            {
                RuleFor(p => p.Email2)
                .MaximumLength(200)
                .WithMessage("O email 2 deve ter no máximo 200 caracteres")
                .EmailAddress()
                .WithMessage("Email 2 inválido")
                .When(p => !string.IsNullOrEmpty(p.Email2));
            });

            When(p => !string.IsNullOrEmpty(p.Email3!), () =>
            {
                RuleFor(p => p.Email2)
                .MaximumLength(200)
                .WithMessage("O email 3 deve ter no máximo 200 caracteres")
                .EmailAddress()
                .WithMessage("Email 3 inválido")
                .When(p => !string.IsNullOrEmpty(p.Email3));
            });
        }
    }
}
