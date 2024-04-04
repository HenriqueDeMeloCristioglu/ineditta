using FluentValidation;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Aprovar
{
    public class AprovarClausulaGeralRequestValidator : AbstractValidator<AprovarClausulaGeralRequest>
    {
        public AprovarClausulaGeralRequestValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .WithMessage("Id não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("O Id deve ser maior que 0");
        }
    }
}
