using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Bus
{
    public interface IRequestHandler<T> where T : Message
    {
        ValueTask<Result> Handle(T message, CancellationToken cancellationToken = default);
    }
}
