using FluentValidation;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Upsert
{
    public class UpsertDocumentoSindicalIARequestValidator : AbstractValidator<UpsertDocumentoSindicalIARequest>
    {
        public UpsertDocumentoSindicalIARequestValidator()
        {
            RuleFor(d => d.DocumentoReferenciaId)
                .NotEmpty()
                .WithMessage("Id do Documento Referência não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do Documento Referência deve ser maior que 0");

            RuleFor(d => d.Status)
                .NotEmpty()
                .WithMessage("Status não pode ser nulo");
        }
    }
}
