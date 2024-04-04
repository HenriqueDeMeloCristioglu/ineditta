using Azure.AI.OpenAI;
using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.Core.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using ChatRole = Ineditta.Application.AIs.Core.Enums.ChatRole;

namespace Ineditta.Integration.AI.Providers.ChatGPT.Factories
{
    public static class ChatGptFactory
    {
        public static Result<IList<ChatRequestMessage>, Error> CreateMessage(SendMessageDto messageList)
        {
            var messages = new List<ChatRequestMessage>();

            if (string.IsNullOrEmpty(messageList.SystemDescription))
            {
                messages.Add(new ChatRequestSystemMessage("Você é um assistente prestativo."));
            } else
            {
                messages.Add(new ChatRequestSystemMessage(messageList.SystemDescription));
            }

            if (messageList?.MessagesExamples?.Any() ?? false)
            {
                foreach (var message in messageList.MessagesExamples)
                {
                    var mensagem = CreateRequestMessage(message.ChatRole, message.Message);
                    if (mensagem.IsFailure)
                    {
                        return Result.Failure<IList<ChatRequestMessage>, Error>(mensagem.Error);
                    }

                    messages.Add(mensagem.Value);
                }
            }

            if (string.IsNullOrEmpty(messageList?.Question))
            {
                return Result.Failure<IList<ChatRequestMessage>, Error>(Errors.General.Business("Nenhuma pergunta foi informada para a IA."));
            }
            else
            {
                var mensagem = CreateRequestMessage(ChatRole.User, messageList.Question);
                if (mensagem.IsFailure)
                {
                    return Result.Failure<IList<ChatRequestMessage>, Error>(mensagem.Error);
                }

                messages.Add(mensagem.Value);
            }

            return Result.Success<IList<ChatRequestMessage>, Error>(messages);
        }

        private static Result<ChatRequestMessage, Error> CreateRequestMessage(ChatRole chatRole, string message)
        {
            ChatRequestMessage chatRequestMessage;

            if (string.IsNullOrEmpty(message))
            {
                return Result.Failure<ChatRequestMessage, Error>(Errors.General.Business("Mensagem informado está nula ou em branco."));
            }

            if (!Enum.IsDefined(typeof(ChatRole), chatRole))
            {
                return Result.Failure<ChatRequestMessage, Error>(Errors.General.Business("Chat role informado é inválido."));
            }

            switch (chatRole)
            {
                case ChatRole.System:
                    chatRequestMessage = new ChatRequestSystemMessage(message);
                    break;
                case ChatRole.User:
                    chatRequestMessage = new ChatRequestUserMessage(message);
                    break;
                case ChatRole.Assistant:
                    chatRequestMessage = new ChatRequestAssistantMessage(message);
                    break;
                default:
                    return Result.Failure<ChatRequestMessage, Error>(Errors.General.Business("Chat Role informado não foi encontrado."));

            }

            return Result.Success<ChatRequestMessage, Error>(chatRequestMessage);
        }
    }
}
