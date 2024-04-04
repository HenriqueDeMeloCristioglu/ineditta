using FluentValidation;

namespace Ineditta.Application.IndicadoresEconomicos.UseCases.Upsert
{
    public class UpsertIndicadorEconomicoRequestValidator : AbstractValidator<UpsertIndicadorEconomicoRequest>
    {
        public UpsertIndicadorEconomicoRequestValidator()
        {
            RuleFor(p => p.Origem)
                .NotEmpty()
                .WithMessage("Informe a origem")
                .MaximumLength(100)
                .WithMessage("Origem deve ter no máximo 100 caracteres");

            RuleFor(p => p.Indicador)
                .NotEmpty()
                .WithMessage("Informe o indicador")
                .MaximumLength(50)
                .WithMessage("Indicador deve ter no máximo 50 caracteres");

            RuleFor(p => p.IdUsuario)
                .GreaterThan(0)
                .WithMessage("Informe o Id do usuário");

            RuleFor(p => p.Fonte)
                .NotEmpty()
                .WithMessage("Informe a fonte")
                .MaximumLength(500)
                .WithMessage("Fonte deve ter no máximo 500 caracteres");

            RuleFor(p => p.PeriodosProjetados)
                .NotEmpty()
                .WithMessage("Informe um período com dado projetado");
        }
    }
}
