using CSharpFunctionalExtensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ineditta.BuildingBlocks.Core.Bus
{
    public class MessageSubscriber : IMessageSubscriber
    {
        private readonly IServiceScopeFactory _serviceScope;
        private readonly ILogger<MessageSubscriber> _logger;
        public MessageSubscriber(ILogger<MessageSubscriber> logger, IServiceScopeFactory serviceScope)
        {
            _logger = logger;
            _serviceScope = serviceScope;
        }

        public async ValueTask<Result> ProcessAsync<T>(T message, CancellationToken cancellationToken = default) where T: Message
        {
            using var scope = _serviceScope.CreateScope();

            try
            {
                var genericHandlerType = typeof(IRequestHandler<>).MakeGenericType(typeof(T));
                var handler = scope.ServiceProvider.GetService(genericHandlerType);

                if (handler is null)
                {
                    _logger.LogError("Handler not found");

                    return Result.Failure("Handler not found");
                }

                return await ((IRequestHandler<T>)handler).Handle(message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure to process message");

                return Result.Failure("Failure to process message");
            }
        }
    }
}
