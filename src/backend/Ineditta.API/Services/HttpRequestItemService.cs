using Ineditta.API.Constants;
using Ineditta.Application.Usuarios.Entities;

namespace Ineditta.API.Services
{
    public class HttpRequestItemService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpRequestItemService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void IncluirUsuario(Usuario usuario)
        {
            _httpContextAccessor.HttpContext!.Items[ApiConstant.Usuario] = usuario;
        }

        public Usuario? ObterUsuario()
        {
            var usuario = _httpContextAccessor.HttpContext!.Items[ApiConstant.Usuario];

            return usuario is null ? default : (Usuario)usuario;
        }
    }
}
