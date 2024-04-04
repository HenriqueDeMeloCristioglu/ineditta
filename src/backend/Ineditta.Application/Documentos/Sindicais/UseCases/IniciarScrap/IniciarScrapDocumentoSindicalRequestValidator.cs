using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.IniciarScrap
{
    public class IniciarScrapDocumentoSindicalRequestValidator : AbstractValidator<IniciarScrapDocumentoSindicalRequest>
    {
        public IniciarScrapDocumentoSindicalRequestValidator()
        {
            RuleFor(x => x.DocumentoId)
                .GreaterThan(0)
                .WithMessage("Você precisa fornecer um id maior que 0");
        }
    }
}
