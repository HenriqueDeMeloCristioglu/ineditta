using FluentValidation;

namespace Ineditta.Application.AIs.Clausulas.UseCases.AtualizarStatus
{
    public class AtualizarStatusRequestValidator : AbstractValidator<AtualizarStatusRequest>
    {
        public AtualizarStatusRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id não pode ser nullo")
                .GreaterThan(0)
                .WithMessage("Id deve ser maior que 0");

            RuleFor(x => x.Status)
                .NotEmpty()
                .WithMessage("Forneça um status");
        }
    }
}
