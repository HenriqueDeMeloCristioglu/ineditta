using FluentValidation;

namespace Ineditta.Application.Clausulas.Gerais.UseCases.Resumir
{
    public class ResumirClausulaRequestValidator : AbstractValidator<ResumirClausulaRequest>
    {
        public ResumirClausulaRequestValidator()
        {
            RuleFor(c => c.DocumentoId)
                .NotEmpty()
                .WithMessage("Id do Documento não pode ser nulo")
                .GreaterThan(0)
                .WithMessage("Id do Documento deve ser maio que 0");
        }
    }
}
