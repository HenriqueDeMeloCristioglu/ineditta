using FluentValidation;

namespace Ineditta.Application.Acompanhamentos.Ccts.UseCases.AdicionarScript
{
    public class AdicionarScriptRequestValidator : AbstractValidator<AdicionarScriptRequest>
    {
        public AdicionarScriptRequestValidator()
        {
            RuleFor(a => a.AcompanhamentosCctsIds)
                .NotNull()
                .WithMessage("Ids dos Acompanhamentos não podem ser nulos");

            RuleFor(a => a.Respostas)
                .NotNull()
                .WithMessage("As Respostas não pode ser nula");

            RuleFor(a => a.StatusId)
                .NotNull()
                .WithMessage("O Id do Status não pode ser nula");

            RuleFor(a => a.FaseId)
                .NotNull()
                .WithMessage("O Id da Fase não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id da Fase deve ser maior que 0");
        }
    }
}
