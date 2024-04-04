using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.Liberar
{
    public class LiberarDocumentoSindicalRequestValidator : AbstractValidator<LiberarDocumentoSindicalRequest>
    {
        public LiberarDocumentoSindicalRequestValidator()
        {
            RuleFor(d => d.Id)
                .NotEmpty()
                .WithMessage("Informe o Id do documento")
                .GreaterThan(0)
                .WithMessage("Id do documento tem que ser maior que 0");
        }
    }
}
