using FluentValidation;

namespace Ineditta.Application.IndicadoresEconomicos.UseCases.Upsert
{
    public class UpsertIndicadorEconomicoRealRequestValidator : AbstractValidator<UpsertIndicadorEconomicoRealRequest>
    {
        public UpsertIndicadorEconomicoRealRequestValidator()
        {
            RuleFor(p => p.Indicador)
                .NotEmpty()
                .WithMessage("Informe o indicador")
                .MaximumLength(50)
                .WithMessage("Indicador deve ter no máximo 50 caracteres");

            RuleFor(p => p.PeriodosReais)
                .NotEmpty()
                .WithMessage("Informe um período com dado real");
        }
    }
}
