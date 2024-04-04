using FluentValidation;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Reprocessar
{
    public class ReprocessarIADocumentoSindicalRequestValidator : AbstractValidator<ReprocessarIADocumentoSindicalRequest>
    {
        public ReprocessarIADocumentoSindicalRequestValidator()
        {
            RuleFor(x => x.DocumentoId)
                .GreaterThan(0)
                .WithMessage("O id do documento sindical ia deve ser maior que 0");
        }
    }
}
