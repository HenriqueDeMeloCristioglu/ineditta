using CSharpFunctionalExtensions;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Polly;

namespace Ineditta.Integration.Mte.Factories
{
    internal static class RetryPolicyFactory
    {
        internal static async Task<Result<T, Error>> ExecuteWithRetryFailureAsync<T>(Func<Task<Result<T, Error>>> action, int retryCount, Func<Task>? onRetryAction = default)
        {
            var retryPolicy = Policy<Result<T, Error>>
                .HandleResult(result => result.IsFailure && (result.Error == Errors.Http.TooManyRequests() || result.Error == Errors.Http.ErrorCode() || result.Error.Code == Errors.Http.Timeout(string.Empty).Code))
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), onRetry: async (result, timeSpan, attempt, context) =>
                {
                    await (onRetryAction?.Invoke() ?? Task.CompletedTask);
                });

            return await retryPolicy.ExecuteAsync(action);
        }
    }
}
