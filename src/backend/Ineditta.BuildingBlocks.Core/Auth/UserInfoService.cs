using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace Ineditta.BuildingBlocks.Core.Auth
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInfoService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetEmail()
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirst(x => ClaimTypes.Email == x.Type)?.Value;
        }
    }
}
