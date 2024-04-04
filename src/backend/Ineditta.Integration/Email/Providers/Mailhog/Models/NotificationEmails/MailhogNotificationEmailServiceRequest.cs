namespace Ineditta.Integration.Email.Providers.Mailhog.Models.NotificationEmailStorage
{
    public class MailhogNotificationEmailServiceRequest
    {
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public string MessageId { get; set; } = null!;
        public string Assunto { get; set; } = null!;
        public bool? Enviado { get; set; }
    }
}
