using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Database
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
        ValueTask<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> action, CancellationToken cancellationToken = default);
        ValueTask<Result> ExecuteInTransactionAsync(Func<Task<Result>> action, CancellationToken cancellationToken = default);
    }
}
