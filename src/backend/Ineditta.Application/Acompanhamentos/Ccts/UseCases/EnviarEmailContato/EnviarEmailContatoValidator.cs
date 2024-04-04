using FluentValidation;

namespace Ineditta.Application.AcompanhamentosCcts.UseCases.EnviarEmail
{
    public class EnviarEmailContatoValidator : AbstractValidator<EnviarEmailContatoRequest>
    {
        public EnviarEmailContatoValidator()
        {
            RuleFor(x => x.Emails)
                .Cascade(CascadeMode.Stop)
                .Must(x => x != null && x.Any())
                .WithMessage("Informe os e-mails");

            RuleForEach(x => x.Emails)
                .NotEmpty()
                .WithMessage("Informe o e-mail")
                .EmailAddress()
                .WithMessage("E-mail inválido");

            RuleFor(p => p.Template)
                .NotEmpty()
                .WithMessage("Informe o template");

            RuleFor(p => p.Assunto)
                .NotEmpty()
                .WithMessage("Informe o assunto");
        }
    }
}
