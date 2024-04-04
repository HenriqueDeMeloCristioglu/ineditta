using System.ComponentModel;
using Ineditta.Application.AIs.Core.Enums;

namespace Ineditta.Application.AIs.Core.Dtos
{
    public class SendMessageDto
    {
        public SendMessageDto (string systemDescription, string question)
        {
            SystemDescription = systemDescription;
            Question = question;
        }

        [Description("How artificial intelligence should behave, respond, and any other characteristic you may want to define. If no description is provided, the predefined 'SYSTEM' role will be used.")]
        public string SystemDescription { get; set; }

        [Description("List of messages alternating between 'USER' and 'SYSTEM,' ending with the last one being 'SYSTEM.' If no message is provided, a direct question will be asked.")]
        public IEnumerable<MessageDto>? MessagesExamples { get; set; } = null!;

        [Description("A question you want to ask the AI.")]
        public string Question { get; set; }
    }

    public class MessageDto
    {
        [Description("Role assigned to the message.")]
        public ChatRole ChatRole { get; set; }

        [Description("When the role is SYSTEM, it serves as a description. When the role is USER, it serves as a question. And when the role is ASSISTANT, it serves as an answer.")]
        public string Message { get; set; } = null!;
    }
}
