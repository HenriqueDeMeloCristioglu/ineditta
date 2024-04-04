using Ineditta.BuildingBlocks.Core.Database;

using Microsoft.EntityFrameworkCore.Storage;

namespace Ineditta.BuildingBlocks.Core.Domain.Handlers
{
    public abstract class BaseCommandHandler
    {
#pragma warning disable CA1051 // Do not declare visible instance fields
        protected readonly IUnitOfWork _unitOfWork;
#pragma warning restore CA1051 // Do not declare visible instance fields

        protected BaseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected async ValueTask<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
