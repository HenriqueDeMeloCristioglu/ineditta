using Ineditta.API.Services;
using Ineditta.Application.Usuarios.Repositories;
using Ineditta.BuildingBlocks.Core.Auth;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Ineditta.API.Filters
{
    public class UserInfoActionFilter : IAsyncActionFilter
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly HttpRequestItemService _httpRequestItemService;

        public UserInfoActionFilter(IUserInfoService userInfoService, IUsuarioRepository usuarioRepository, HttpRequestItemService httpRequestItemService)
        {
            _userInfoService = userInfoService;
            _usuarioRepository = usuarioRepository;
            _httpRequestItemService = httpRequestItemService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var email = _userInfoService.GetEmail();
            if (string.IsNullOrEmpty(email))
            {
                return;
            }

            var usuario = await _usuarioRepository.ObterPorEmailAsync(email);
            if (usuario == null)
            {
                return;
            }

            _httpRequestItemService.IncluirUsuario(usuario);

            await next();
        }
    }
}
