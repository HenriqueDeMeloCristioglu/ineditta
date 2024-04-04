using Microsoft.EntityFrameworkCore;

namespace Ineditta.BuildingBlocks.Core.Idempotency.Database
{
    public sealed class IdempotentRepository : IIdempotentRepository
    {
        private readonly DbContext _context;

        public IdempotentRepository(DbContext context)
        {
            _context = context;
        }

        public async ValueTask AddAsync(IdempotentModel idempotentModel, CancellationToken cancellationToken = default)
        {
            _context.Set<IdempotentModel>().Add(idempotentModel);

            await Task.CompletedTask;
        }

        public async ValueTask<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<IdempotentModel>().AnyAsync(p => p.Id == id, cancellationToken: cancellationToken);
        }
    }
}
