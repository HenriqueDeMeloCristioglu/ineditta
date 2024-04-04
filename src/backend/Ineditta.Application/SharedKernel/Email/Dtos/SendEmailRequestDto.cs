namespace Ineditta.Integration.Email.Dtos
{
    public class SendEmailRequestDto
    {
        public SendEmailRequestDto(IEnumerable<string> to, string subject, string body)
        {
            To = to;
            IsHtml = true;
            Subject = subject;
            Body = body;
        }

        public IEnumerable<string> To { get; set; }
        public IEnumerable<string>? Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
    }
}
