using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Emails.CaixasDeSaida.Entities
{
    public class EmailCaixaDeSaida : Entity
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected EmailCaixaDeSaida() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private EmailCaixaDeSaida(Email email, string assunto, string template, DateOnly dataInclusao, string messageId)
        {
            Email = email;
            Assunto = assunto;
            Template = template;
            DataInclusao = dataInclusao;
            MessageId = messageId;
        }

        public Email Email { get; private set; }
        public string Assunto { get; private set; }
        public string Template { get; private set; }
        public DateOnly DataInclusao { get; private set; }
        public string MessageId { get; private set; }

        public static Result<EmailCaixaDeSaida> Criar(Email email, string assunto, string template, string messageId)
        {
            if (email.Valor is null)
            {
                return Result.Failure<EmailCaixaDeSaida>("Email inválido");
            }

            if (assunto is null)
            {
                return Result.Failure<EmailCaixaDeSaida>("Assunto inválido");
            }

            if (template is null)
            {
                return Result.Failure<EmailCaixaDeSaida>("Assunto inválido");
            }

            if (messageId is null)
            {
                return Result.Failure<EmailCaixaDeSaida>("Message Id inválido");
            }

            var emailCaixaDeSaida = new EmailCaixaDeSaida(email, assunto, template, DateOnly.FromDateTime(DateTime.Now), messageId);

            return Result.Success(emailCaixaDeSaida);
        }
    }
}
