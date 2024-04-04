using FluentValidation;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.AtualizarTextoResumido
{
    public class AtualizarTextoResumidoRequestValidator : AbstractValidator<AtualizarTextoResumidoRequest>
    {
        public AtualizarTextoResumidoRequestValidator()
        {
            RuleFor(c => c.DocumentoId)
                .NotEmpty()
                .WithMessage("Id do documento não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do documento deve ser maio que 0");

            RuleFor(c => c.EstruturaId)
                .NotEmpty()
                .WithMessage("Id do documento não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do documento deve ser maio que 0");

            RuleFor(c => c.Texto)
                .NotEmpty()
                .WithMessage("O Texto não pode ser nulo");
        }
    }
}
