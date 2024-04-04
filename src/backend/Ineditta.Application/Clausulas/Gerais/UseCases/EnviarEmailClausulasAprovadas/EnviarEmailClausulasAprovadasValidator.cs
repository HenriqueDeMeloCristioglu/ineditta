
using FluentValidation;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.EnviarEmailClausulasAprovadas
{
    public class EnviarEmailClausulasAprovadasValidator : AbstractValidator<EnviarEmailClausulasAprovadasRequest>
    {
        public EnviarEmailClausulasAprovadasValidator()
        {
            RuleFor(d => d.DocumentoId)
                .GreaterThan(0)
                .WithMessage("O id do documento deve ser maior que 0");
        }
    }
}
