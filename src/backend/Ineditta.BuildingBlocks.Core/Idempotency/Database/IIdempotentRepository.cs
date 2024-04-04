namespace Ineditta.BuildingBlocks.Core.Idempotency.Database
{
    public interface IIdempotentRepository
    {
        ValueTask<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);
        ValueTask AddAsync(IdempotentModel idempotentModel, CancellationToken cancellationToken = default);
    }
}
