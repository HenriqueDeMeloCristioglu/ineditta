using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.Aprovar
{
    public class AprovarDocumentoSindicalRequestValidator : AbstractValidator<AprovarDocumentoSindicalRequest>
    {
        public AprovarDocumentoSindicalRequestValidator()
        {
            RuleFor(p => p.Id)
                .GreaterThan(0)
                .WithMessage("O id deve ser maior que 0");
        }
    }
}
