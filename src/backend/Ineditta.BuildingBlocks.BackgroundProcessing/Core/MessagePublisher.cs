using CSharpFunctionalExtensions;

using Hangfire;

using Ineditta.BuildingBlocks.Core.Bus;

namespace Ineditta.BuildingBlocks.BackgroundProcessing.Core
{
    public class MessagePublisher : IMessagePublisher
    {
        public async ValueTask<Result> SendAsync<T>(T message, CancellationToken cancellationToken) where T : Message
        {
#pragma warning disable CA2012 // Usar ValueTasks corretamente
            BackgroundJob.Enqueue<IMessageSubscriber>(ms => ms.ProcessAsync(message, cancellationToken));
#pragma warning restore CA2012 // Usar ValueTasks corretamente

            return await Task.FromResult(Result.Success());
        }
    }
}
