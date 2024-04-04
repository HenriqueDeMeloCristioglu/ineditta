using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Bus
{
    public interface IMessageSubscriber
    {
        ValueTask<Result> ProcessAsync<T>(T message, CancellationToken cancellationToken = default) where T: Message;
    }
}
