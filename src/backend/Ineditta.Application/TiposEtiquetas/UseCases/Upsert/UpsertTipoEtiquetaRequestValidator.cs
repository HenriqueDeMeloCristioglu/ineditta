using FluentValidation;

namespace Ineditta.Application.TiposEtiquetas.UseCases.Upsert
{
    public class UpsertTipoEtiquetaRequestValidator : AbstractValidator<UpsertTipoEtiquetaRequest>
    {
        public UpsertTipoEtiquetaRequestValidator()
        {
            RuleFor(t => t.Nome)
                .NotEmpty()
                .WithMessage("Campo Nome não pode ser nulo");

            RuleFor(t => t.Id)
                .NotEmpty()
                .WithMessage("Campo Id não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id inválido");
        }
    }
}
