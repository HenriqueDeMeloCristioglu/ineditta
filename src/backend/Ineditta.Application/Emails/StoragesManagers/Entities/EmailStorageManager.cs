using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;

namespace Ineditta.Application.Emails.StoragesManagers.Entities
{
    public class EmailStorageManager : Entity<long>
    {
        public EmailStorageManager(Email from, Email to, string messageId, string assunto, DateTime dataInclusao, bool enviado, string requestData)
        {
            From = from;
            To = to;
            MessageId = messageId;
            Assunto = assunto;
            DataInclusao = dataInclusao;
            Enviado = enviado;
            RequestData = requestData;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected EmailStorageManager()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        public Email From { get; private set; }
        public Email To { get; private set; }
        public string MessageId { get; private set; }
        public string Assunto { get; private set; }
        public DateTime DataInclusao { get; private set; }
        public bool Enviado { get; private set; }
        public string RequestData { get; private set; }

        public static Result<EmailStorageManager> Criar(Email from, Email to, string messageId, string assunto, bool enviado, string requestData)
        {
            if (from.Valor is null)
            {
                return Result.Failure<EmailStorageManager>("Email Remetente inválido");
            }

            if (to.Valor is null)
            {
                return Result.Failure<EmailStorageManager>("Email Destinatário inválido");
            }

            if (messageId is null)
            {
                return Result.Failure<EmailStorageManager>("Message Id inválido");
            }

            if (assunto is null)
            {
                return Result.Failure<EmailStorageManager>("Assunto inválido");
            }

            if (requestData is null)
            {
                return Result.Failure<EmailStorageManager>("Dados da Request inválidos");
            }

            var emailStorageManager = new EmailStorageManager(from, to, messageId, assunto, DateTime.Now, enviado, requestData);

            return Result.Success(emailStorageManager);
        }
    }
}
