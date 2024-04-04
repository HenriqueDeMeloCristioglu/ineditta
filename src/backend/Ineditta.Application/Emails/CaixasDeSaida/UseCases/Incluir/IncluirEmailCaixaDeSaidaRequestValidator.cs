using FluentValidation;

namespace Ineditta.Application.Emails.CaixasDeSaida.UseCases.Incluir
{
    public class IncluirEmailCaixaDeSaidaRequestValidator : AbstractValidator<IncluirEmailCaixaDeSaidaRequest>
    {
        public IncluirEmailCaixaDeSaidaRequestValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty()
                .WithMessage("O Email não pode ser nulo");

            RuleFor(e => e.Assunto)
                .NotEmpty()
                .WithMessage("O Assunto não pode ser nulo");

            RuleFor(e => e.Template)
                .NotEmpty()
                .WithMessage("O Template não pode ser nulo");

            RuleFor(e => e.MessageId)
                .NotEmpty()
                .WithMessage("O MessageId não pode ser nulo");
        }
    }
}
