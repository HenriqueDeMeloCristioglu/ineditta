using System.Diagnostics;

using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

using Microsoft.EntityFrameworkCore;

namespace Ineditta.Repository.Contexts
{
    public class ResilientTransaction
    {
        private readonly DbContext _context;
        private ResilientTransaction(DbContext context) =>
            _context = context ?? throw new ArgumentNullException(nameof(context));

        public static ResilientTransaction New(DbContext context) =>
            new(context);

        public async Task<Result> ExecuteAsync(Func<Task<Result>> action)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                var result = await action();

                if (result.IsSuccess)
                {
                    await transaction.CommitAsync();
                    return result;
                }

                await transaction.RollbackAsync();

                return result;
            });
        }

        public async Task<Result<T, Error>> ExecuteAsync<T>(Func<Task<Result<T, Error>>> action)
        {
            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    var result = await action();

                    if (result.IsSuccess)
                    {
                        await transaction.CommitAsync();
                        return result;
                    }

                    await transaction.RollbackAsync();

                    return result;
                });
            }
            catch (DbUpdateException dbex)
            {
                Debug.WriteLine(dbex);

                return Result.Failure<T, Error>(Errors.General.InternalServerError("Ocorreu uma exceção ao salvar os registros"));
            }
        }

        public async Task<Result<TResult>> ExecuteAsync<TResult>(Func<Task<Result<TResult>>> action)
        {
            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    var result = await action();

                    if (result.IsSuccess)
                    {
                        await transaction.CommitAsync();
                        return result;
                    }

                    await transaction.RollbackAsync();

                    return result;
                });
            }
            catch (DbUpdateException dbex)
            {
                Debug.WriteLine(dbex);

                return Result.Failure<TResult>("Ocorre uma falha ao salvar os registros");
            }
        }
    }
}