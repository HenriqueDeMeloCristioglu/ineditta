using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.UpsertComercial
{
    public class UpsertComercialDocumentoSindicalRequestValidator : AbstractValidator<UpsertComercialDocumentoSindicalRequest>
    {
        public UpsertComercialDocumentoSindicalRequestValidator()
        {
            When(p => p.IdDocSind is not null, () =>
            {
                RuleFor(p => p.IdDocSind)
                    .GreaterThan(0)
                    .WithMessage("O id do documento deve ser maior que 0");
            });

            When(p => p.SindLaboral is not null && p.SindLaboral.Any(), () =>
            {
                RuleFor(p => p.SindLaboral)
                    .Must(p => p is not null && p.Any(a => a >= 0))
                    .WithMessage("Não pode haver ids de sindicatos laborais menor ou igual a 0");
            });

            When(p => p.SindPatronal is not null && p.SindPatronal.Any(), () =>
            {
                RuleFor(p => p.SindPatronal)
                    .Must(p => p is not null && p.Any(a => a >= 0))
                    .WithMessage("Não pode haver ids de sindicatos patronais menor ou igual a 0");
            });
        }
    }
}
