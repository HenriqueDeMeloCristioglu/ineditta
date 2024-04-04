using Ineditta.Integration.Email.Providers.Aws.Converters;

using Newtonsoft.Json;

namespace Ineditta.Integration.Email.Providers.Aws.Models
{
    public class AmazonSesNotificationRequest
    {
        public string MessageId { get; set; } = null!;

        [JsonConverter(typeof(StringToJsonConverter<Message>))]
        public Message Message { get; set; } = null!;
        public DateTimeOffset Timestamp { get; set; }
        public string Signature { get; set; } = null!;
    }

    public class Mail
    {
        public string? Timestamp { get; set; }
        public string? Source { get; set; }
        public string? SourceArn { get; set; }
        public string? SourceIp { get; set; }
        public string? CallerIdentity { get; set; }
        public string? SendingAccountId { get; set; }
        public string MessageId { get; set; } = null!;
        public List<string> Destination { get; set; } = null!;
        public bool HeadersTruncated { get; set; }
        public List<Header>? Headers { get; set; }
        public CommonHeaders? CommonHeaders { get; set; }
    }

    public class Header
    {
        public string Name { get; set; } = null!;
        public string Value { get; set; } = null!;
    }

    public class CommonHeaders
    {
        public List<string> From { get; set; } = null!;
        public List<string> To { get; set; } = null!;
        public string? Subject { get; set; }
    }

    public class Delivery
    {
        public string? Timestamp { get; set; }
        public int ProcessingTimeMillis { get; set; }
        public List<string>? Recipients { get; set; }
        public string? SmtpResponse { get; set; }
        public string? RemoteMtaIp { get; set; }
        public string? ReportingMTA { get; set; }
    }

    public class Message
    {
        public string NotificationType { get; set; } = null!;
        public Mail Mail { get; set; } = null!;
        public Delivery? Delivery { get; set; }
    }

}
