using FluentValidation;

namespace Ineditta.Application.Emails.CaixasDeSaida.UseCases.Deletar
{
    public class DeletarEmailCaixaDeSaidaRequestValidator : AbstractValidator<DeletarEmailCaixaDeSaidaRequest>
    {
        public DeletarEmailCaixaDeSaidaRequestValidator()
        {
            RuleFor(e => e.EmailCaixaDeSaida)
                .NotEmpty()
                .WithMessage("EmailCaixaDeSaida não pode ser vazio");
        }
    }
}
