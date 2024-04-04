namespace Ineditta.Integration.AI.Configurations
{
    public class AIConfiguration
    {
        public ChatGptConfiguration ChatGPT { get; set; } = null!;
    }

    public class ChatGptConfiguration
    {
        public string ApiKey { get; set; } = null!;
        public string Model { get; set; } = null!;
    }
}
