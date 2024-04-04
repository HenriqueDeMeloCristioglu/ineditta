using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Database;

using Microsoft.EntityFrameworkCore.Storage;

namespace Ineditta.Repository.Contexts
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InedittaDbContext _context;
        
        public UnitOfWork(InedittaDbContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        
#pragma warning disable IDE0060 // Remove unused parameter
        public async ValueTask<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> action, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return await ResilientTransaction.New(_context).ExecuteAsync<T>(action);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public async ValueTask<Result> ExecuteInTransactionAsync(Func<Task<Result>> action, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return await ResilientTransaction.New(_context).ExecuteAsync(action);
        }
    }
}
