using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.UpsertSisap
{
    public class UpsertSisapDocumentoSindicalRequestValidator : AbstractValidator<UpsertSisapDocumentoSindicalRequest>
    {
        public UpsertSisapDocumentoSindicalRequestValidator()
        {
            When(p => p.IdDocSind is not null, () =>
            {
                RuleFor(p => p.IdDocSind)
                    .GreaterThan(0)
                    .WithMessage("O id do documento deve ser maior que 0");
            });

            RuleFor(p => p.IdDocumento)
                .GreaterThan(0)
                .WithMessage("O id do documento localizado deve ser maior que 0");

            RuleFor(p => p.Abrangencia)
                .Must(p => p is not null && p.Any())
                .WithMessage("Você precisa fornecer a abrangência do documento.")
                .Must(p => p is not null && p.Any(a => a >= 0))
                .WithMessage("Não pode haver ids de abrangências menor ou igual a 0");

            RuleFor(p => p.CnaesIds)
                .Must(p => p is not null && p.Any())
                .WithMessage("Você precisa fornecer as atividades econômicas do documento")
                .Must(p => p is not null && p.Any(a => a >= 0))
                .WithMessage("Não pode haver ids de atividades econômicas menor ou igual a 0");

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

            RuleFor(p => p.DataBase)
                .NotEmpty()
                .WithMessage("Você deve fornecer uma data-base");
        }
    }
}
