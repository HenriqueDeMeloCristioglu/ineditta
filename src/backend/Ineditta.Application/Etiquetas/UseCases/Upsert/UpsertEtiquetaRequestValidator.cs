using FluentValidation;

namespace Ineditta.Application.Etiquetas.UseCases.Upsert
{
    public class UpsertEtiquetaRequestValidator : AbstractValidator<UpsertEtiquetaRequest>
    {
        public UpsertEtiquetaRequestValidator()
        {
            RuleFor(e => e.Id)
                .NotEmpty()
                .WithMessage("Campo Id não pode ser vazio")
                .GreaterThan(0)
                .WithMessage("Campo Id inválido");

            RuleFor(e => e.Nome)
                .NotEmpty()
                .WithMessage("Campo Nome não pode ser vazio");

            RuleFor(e => e.TipoEtiquetaId)
                .NotEmpty()
                .WithMessage("Campo Tipo de etiqueta não pode ser vazio")
                .GreaterThan(0)
                .WithMessage("Campo Tipo de etiqueta inválido");
        }
    }
}
