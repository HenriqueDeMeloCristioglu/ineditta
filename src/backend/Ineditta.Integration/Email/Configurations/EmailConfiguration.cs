namespace Ineditta.Integration.Email.Configurations
{
    public class EmailConfiguration
    {
        public MailhogSmtpConfiguration Smtp { get; set; } = null!;
        public AwsConfiguration Aws { get; set; } = null!;
    }
    public class MailhogSmtpConfiguration
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FromEmail { get; set; } = null!;
    }

    public class AwsConfiguration
    {
        public string Source { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
    }
}
