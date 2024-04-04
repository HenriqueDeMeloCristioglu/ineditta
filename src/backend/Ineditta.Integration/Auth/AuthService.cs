using System.Globalization;

using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;

using Ineditta.Application.SharedKernel.Auth;
using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Domain.ValueObjects;
using Ineditta.Integration.Auth.Dtos;
using Ineditta.Integration.Auth.Erros;

namespace Ineditta.Integration.Auth
{
    public class AuthService : IAuthService
    {
        private readonly KeycloakHttpClient _keycloakHttpClient;

        public AuthService(KeycloakHttpClient keycloakHttpClient)
        {
            _keycloakHttpClient = keycloakHttpClient;
        }

        private async ValueTask<bool> ExistsByEmail(string email)
        {
            var result = await _keycloakHttpClient.GetAsync<FindUserDto, IEnumerable<FindUserResponseDto>>("/users", new FindUserDto { Email = email, Exact = true });

            return result.IsSuccess && result.Value is not null && result.Value.Any();
        }

        private async ValueTask<bool> ExistsByUsername(string username)
        {
            var result = await _keycloakHttpClient.GetAsync<FindUserDto, IEnumerable<FindUserResponseDto>>("/users", new FindUserDto { Username = username, Exact = true });

            return result.IsSuccess && result.Value is not null && result.Value.Any();
        }

        public async ValueTask<Result<bool, Error>> IncluirAsync(Usuario usuario, string username)
        {
            if (await ExistsByEmail(usuario.Email))
            {
                return Result.Failure<bool, Error>(AuthErrors.DuplicatedEmail());
            }

            if (await ExistsByUsername(username))
            {
                return Result.Failure<bool, Error>(AuthErrors.DuplicatedUsername());
            }

            var createUserDto = Convert(usuario, username);

            var result = await _keycloakHttpClient.PostAndReceiveHeadersAsync("/users", createUserDto);

            if (result.IsFailure)
            {
                return Result.Failure<bool, Error>(result.Error);
            }

            var location = result.Value!.SingleOrDefault(x => x.Key.Equals("Location", StringComparison.OrdinalIgnoreCase)).Value?.FirstOrDefault();

            if (string.IsNullOrEmpty(location))
            {
                return Result.Failure<bool, Error>(AuthErrors.UserNotFound());
            }

            var userIdString = location.Substring(location.LastIndexOf("/", StringComparison.InvariantCulture) + 1);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Result.Failure<bool, Error>(AuthErrors.UserNotFound());
            }

            return await AtualizarPermissoesAsync(userId);
        }

        public ValueTask<Result<bool, Error>> AtualizarAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        private static CreateUserDto Convert(Usuario usuario, string username)
        {
#pragma warning disable S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
            return new CreateUserDto
            {
                CreatedTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Email = usuario.Email,
                FirstName = usuario.Nome.Split(" ").First(),
                LastName = usuario.Nome.Split(" ").Last(),
                Username = username,
                Totp = false,
                Enabled = true,
                EmailVerified = false,
                NotBefore = 0,
                Access = new AccessDto
                {
                    ManageGroupMembership = true,
                    MapRoles = true,
                    Impersonate = true,
                    Manage = true
                },
                Attributes = new AttributesDto
                {
                    IdFilial = usuario.EstabelecimentosIds == null ? default : string.Join(", ", usuario.EstabelecimentosIds),
                    Tipo = usuario.Tipo
                },
                Credentials = new List<CredentialsDto>
                {
                    new CredentialsDto
                    {
                        Type = "password",
                        Value = "Ineditta123@",
                        Temporary = true
                    }
                }
            };
#pragma warning restore S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
        }

        public async ValueTask<Result<bool, Error>> EnviarEmailAtualizacaoCredenciaisAsync(Usuario usuario)
        {
            var authUser = await _keycloakHttpClient.GetAsync<FindUserDto, IEnumerable<FindUserResponseDto>>("/users", new FindUserDto { Email = usuario.Email });

            if (authUser.IsFailure)
            {
                return Result.Failure<bool, Error>(AuthErrors.UserNotFound());
            }

            if (authUser.Value is null || !authUser.Value.Any())
            {
                return Result.Failure<bool, Error>(AuthErrors.UserNotFound());
            }

            var envioEmailResult = await _keycloakHttpClient.PutAsync($"/users/{authUser.Value!.First().Id}/execute-actions-email", new List<string> { "UPDATE_PASSWORD", "UPDATE_PROFILE" });

            return envioEmailResult.IsFailure
                ? Result.Failure<bool, Error>(AuthErrors.EmailUpdateCredential())
                : Result.Success<bool, Error>(true);
        }

        public async ValueTask<Result<UsuarioAuthDto, Error>> ObterPorEmailAsync(string email)
        {
            var result = await _keycloakHttpClient.GetAsync<FindUserDto, IEnumerable<FindUserResponseDto>>("/users", new FindUserDto { Email = email, Exact = true });

            if (result.IsFailure)
            {
                return Result.Failure<UsuarioAuthDto, Error>(result.Error);
            }

            return result.Value is null || !result.Value.Any()
                ? Result.Failure<UsuarioAuthDto, Error>(AuthErrors.UserNotFound())
                : Result.Success<UsuarioAuthDto, Error>(new UsuarioAuthDto { Email = result.Value.First().Email, Username = result.Value.First().Username, Id = result.Value.First().Id });
        }

        public async ValueTask<Result<bool, Error>> AtualizarPermissoesAsync(Usuario usuario)
        {
            var authUser = await _keycloakHttpClient.GetAsync<FindUserDto, IEnumerable<FindUserResponseDto>>("/users", new FindUserDto { Email = usuario.Email });

            if (authUser.IsFailure)
            {
                return Result.Failure<bool, Error>(authUser.Error);
            }

            if (authUser.Value is null || !authUser.Value.Any())
            {
                return Result.Failure<bool, Error>(AuthErrors.UserNotFound());
            }

            var result = await _keycloakHttpClient.PutAsync($"/users/{authUser.Value!.First().Id}/groups/3911e17f-96f2-43d6-a950-bc93876fe3d4");

            return result.IsSuccess ? Result.Success<bool, Error>(true) : Result.Failure<bool, Error>(result.Error);
        }


        private async ValueTask<Result<bool, Error>> AtualizarPermissoesAsync(Guid usuarioId)
        {
            var result = await _keycloakHttpClient.PutAsync($"/users/{usuarioId}/groups/3911e17f-96f2-43d6-a950-bc93876fe3d4");

            return result.IsSuccess ? Result.Success<bool, Error>(true) : Result.Failure<bool, Error>(result.Error);
        }
    }
}
