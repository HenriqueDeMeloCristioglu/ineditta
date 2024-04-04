using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.Usuarios.Services
{
    public interface IUsuarioEmailService
    {
        ValueTask<Result<bool, Error>> EnviarBoasVindasAsync(string email, CancellationToken cancellationToken = default);
    }
}
