using Azure.AI.OpenAI;
using Azure;
using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.Core.Services;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.Integration.AI.Configurations;
using Microsoft.Extensions.Options;
using Ineditta.Application.AIs.Core.Dtos;
using Ineditta.Integration.AI.Providers.ChatGPT.Factories;
using Microsoft.FeatureManagement;
using Ineditta.Application.SharedKernel.FeaturesFlags;
using CSharpFunctionalExtensions.ValueTasks;

namespace Ineditta.Integration.AI.Providers.ChatGPT.Services
{
    public class ChatGptService : IAIService
    {
        private readonly AIConfiguration _configuration;
        private readonly IFeatureManager _featureManager;
        private const int RetryCount = 10;

        public ChatGptService(IOptions<AIConfiguration> configuration, IFeatureManager featureManager)
        {
            _configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
            _featureManager = featureManager;
        }

        public async ValueTask<Result<string, Error>> RealizarPergunta(SendMessageDto message, CancellationToken cancellationToken = default)
        {
            return await RetryPolicyFactory.ExecuteWithRetryFailureAsync(async () =>
            {
                if (!await _featureManager.IsEnabledAsync(nameof(FeatureFlag.UtilizaIA)))
                {
                    return Result.Success<string, Error>($"Feature flag de comunicação com a IA está desabilitado. {message.Question}");
                }

                var client = new OpenAIClient(_configuration.ChatGPT.ApiKey, new OpenAIClientOptions());

                var messageCreated = ChatGptFactory.CreateMessage(message);

                if (messageCreated.IsFailure)
                {
                    return Result.Failure<string, Error>(messageCreated.Error);
                }

                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = _configuration.ChatGPT.Model,
                    ChoiceCount = 1
                };

                foreach (var chatMessage in messageCreated.Value)
                {
                    chatCompletionsOptions.Messages.Add(chatMessage);
                }

                try
                {
                    Response<ChatCompletions> response = await client.GetChatCompletionsAsync(chatCompletionsOptions, cancellationToken);
                    ChatResponseMessage responseMessage = response.Value.Choices[0].Message;

                    return Result.Success<string, Error>(responseMessage.Content);
                }
                catch (RequestFailedException e) when (e.Status == 429)
                {
                    return Result.Failure<string, Error>(Errors.Http.TooManyRequests());
                }
                catch (RequestFailedException e) when (e.Status >= 500 && e.Status <= 599)
                {
                    return Result.Failure<string, Error>(Errors.Http.ErrorCode());
                }
                catch (RequestFailedException e)
                {
                    return Result.Failure<string, Error>(Errors.General.InternalServerError($"An error ocurred on chatGPT: {e.ErrorCode} - {e.Message}"));
                }
                catch (Exception ex)
                {
                    return Result.Failure<string, Error>(Errors.General.InternalServerError($"An unexpected error ocurred: {ex.Message}"));
                }
            }, RetryCount);
        }
    }
}
