namespace Ineditta.Integration.OCR.Configurations
{
    public class OcrConfiguration
    {
        public AwsConfiguration Aws { get; set; } = null!;
    }

    public class AwsConfiguration
    {
        public string Source { get; set; } = null!;
        public string AccessKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
    }
}
