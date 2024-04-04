using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Bus
{
    public interface IMessagePublisher
    {
        ValueTask<Result> SendAsync<T>(T message, CancellationToken cancellationToken) where T : Message;
    }
}
