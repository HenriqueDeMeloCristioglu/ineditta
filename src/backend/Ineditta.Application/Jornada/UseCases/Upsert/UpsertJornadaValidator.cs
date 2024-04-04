using FluentValidation;

namespace Ineditta.Application.Jornada.UseCases.Upsert
{
    public class UpsertJornadaValidator : AbstractValidator<UpsertJornadaRequest>
    {
        public UpsertJornadaValidator()
        {
            RuleFor(p => p.IsDeault)
                .NotEmpty()
                .WithMessage("Informe o IsDefault");
        }
    }
}
