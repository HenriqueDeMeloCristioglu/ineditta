using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Auth;

namespace Ineditta.Application.Usuarios.Factories
{
    public class ObterUsuarioLogadoFactory
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUserInfoService _userInfoService;

        public ObterUsuarioLogadoFactory(IUsuarioRepository usuarioRepository, IUserInfoService userInfoService)
        {
            _usuarioRepository = usuarioRepository;
            _userInfoService = userInfoService;
        }

        public async Task<Result<Usuario>> PorEmail()
        {
            var email = _userInfoService.GetEmail();

            if (email == null)
            {
                return Result.Failure<Usuario>("Email do usuário inválido");
            }

            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);

            if (usuario == null)
            {
                return Result.Failure<Usuario>("Usuário não encontrado");
            }

            return usuario;
        }
    }
}
