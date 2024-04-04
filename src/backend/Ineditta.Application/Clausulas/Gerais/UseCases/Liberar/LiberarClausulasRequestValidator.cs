using FluentValidation;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Liberar
{
    public class LiberarClausulasRequestValidator : AbstractValidator<LiberarClausulasRequest>
    {
        public LiberarClausulasRequestValidator()
        {
            RuleFor(c => c.DocumentoId)
                .NotEmpty()
                .WithMessage("Id do Documento não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do Documento deve ser maio que 0");
        }
    }
}
