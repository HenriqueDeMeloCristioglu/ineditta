using FluentValidation;

namespace Ineditta.Application.AIs.DocumentosSindicais.UseCases.Aprovar
{
    public class AprovarIADocumentoSindicalRequestValidator : AbstractValidator<AprovarIADocumentoSindicalRequest>
    {
        public AprovarIADocumentoSindicalRequestValidator()
        {
            RuleFor(x => x.DocumentoId)
                .GreaterThan(0)
                .WithMessage("O id do documento sindical ia deve ser maior que 0");
        }
    }
}
