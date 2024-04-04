using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.AtualizarDataSla
{
    public class AtualizarDataSlaRequestValidator : AbstractValidator<AtualizarDataSlaRequest>
    {
        public AtualizarDataSlaRequestValidator()
        {
            RuleFor(p => p.DocSindId)
                .GreaterThan(0)
                .WithMessage("O id fornecido deve ser maior que 0");

        }
    }
}
