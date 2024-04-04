using CSharpFunctionalExtensions;
using Ineditta.Application.AIs.Core.Dtos;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.AIs.Core.Services
{
    public interface IAIService
    {
        ValueTask<Result<string, Error>> RealizarPergunta(SendMessageDto message, CancellationToken cancellationToken = default);
    }
}
