using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.CctsLocalizacoes.UseCases.Upsert
{
    public class IncluirAcompanhamentoCctLocalizacaoRequestValidator : AbstractValidator<IncluirAcompanhamentoCctLocalizacaoRequest>
    {
        public IncluirAcompanhamentoCctLocalizacaoRequestValidator()
        {
            RuleFor(a => a.AcompanhamentoCctId)
                .NotEmpty()
                .WithMessage("O Id do acompanhamento cct não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id do acompanhamento cct deve ser maior que 0");

            RuleFor(a => a.LocalizacaoId)
                .NotEmpty()
                .WithMessage("O Id da localização não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id da localização deve ser maior que 0");
        }
    }
}
