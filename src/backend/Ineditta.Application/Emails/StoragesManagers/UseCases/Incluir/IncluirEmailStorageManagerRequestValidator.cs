using FluentValidation;

using Ineditta.Application.Emails.StoragesManagers.UseCases.Incluir;

namespace Ineditta.Application.Emails.EmailsStoragesManagers.UseCases.Incluir
{
    public class IncluirEmailStorageManagerRequestValidator : AbstractValidator<IncluirEmailStorageManagerRequest>
    {
        public IncluirEmailStorageManagerRequestValidator()
        {
            RuleFor(e => e.From)
                .NotEmpty()
                .WithMessage("O Email do Remetente não pode ser nulos");

            RuleFor(e => e.To)
                .NotEmpty()
                .WithMessage("O Email do Destinatário não pode ser nulos");

            RuleFor(e => e.MessageId)
                .NotEmpty()
                .WithMessage("Message Id não pode ser nulos");

            RuleFor(e => e.Assunto)
                .NotEmpty()
                .WithMessage("O Assunto não pode ser nulos");

            RuleFor(e => e.Enviado)
                .NotEmpty()
                .WithMessage("O Status não pode ser nulos");

            RuleFor(e => e.RequestData)
                .NotEmpty()
                .WithMessage("O Dados da Request não podem ser nulos");
        }
    }
}
