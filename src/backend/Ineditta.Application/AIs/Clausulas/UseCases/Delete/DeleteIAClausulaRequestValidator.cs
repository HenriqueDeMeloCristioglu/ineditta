using FluentValidation;

namespace Ineditta.Application.AIs.Clausulas.UseCases.Delete
{
    public class DeleteIAClausulaRequestValidator : AbstractValidator<DeleteIAClausulaRequest>
    {
        public DeleteIAClausulaRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id não pode ser nullo")
                .GreaterThan(0)
                .WithMessage("Id deve ser maior que 0");
        }
    }
}
