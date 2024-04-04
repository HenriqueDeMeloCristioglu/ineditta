using FluentValidation;

namespace Ineditta.Application.Sindicatos.Patronais.UseCases.Upsert
{
    public class UpsertSindicatoPatronalRequestValidator : AbstractValidator<UpsertSindicatoPatronalRequest>
    {
        public UpsertSindicatoPatronalRequestValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("O id fornecido deve ser maior que 0");

            RuleFor(p => p.Cnpj)
                .NotEmpty()
                .WithMessage("Informe o CNPJ");
        }
    }
}
