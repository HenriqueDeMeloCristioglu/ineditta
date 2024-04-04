using FluentValidation;

namespace Ineditta.Application.Documentos.Sindicais.UseCases.EnviarEmailAprovacao
{
    public class EnviarEmailAprovacaoRequestValidator : AbstractValidator<EnviarEmailAprovacaoRequest>
    {
        public EnviarEmailAprovacaoRequestValidator()
        {
            RuleFor(d => d.DocumentoId)
                .GreaterThan(0)
                .WithMessage("O id do documento deve ser maior que 0");
        }
    }
}
