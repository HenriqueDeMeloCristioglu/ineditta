using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.Application.SharedKernel.Auth
{
    public interface IAuthService
    {
        ValueTask<Result<bool, Error>> IncluirAsync(Usuario usuario, string username);
        ValueTask<Result<bool, Error>> AtualizarAsync(Usuario usuario);
        ValueTask<Result<bool, Error>> EnviarEmailAtualizacaoCredenciaisAsync(Usuario usuario);
        ValueTask<Result<UsuarioAuthDto, Error>> ObterPorEmailAsync(string email);
        ValueTask<Result<bool, Error>> AtualizarPermissoesAsync(Usuario usuario);
    }
}
