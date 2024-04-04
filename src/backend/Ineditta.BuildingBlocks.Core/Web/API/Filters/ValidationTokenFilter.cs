using System.Net;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Tokens;
using Ineditta.BuildingBlocks.Core.Web.API.Actions;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Ineditta.BuildingBlocks.Core.Web.API.Filters
{

    public sealed class ValidationTokenFilter : IAsyncActionFilter
    {
        private readonly ITokenService _tokenService;


        public ValidationTokenFilter(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }


        private static EnvelopeResult Unauthorized() => new(Envelope.Error(Errors.Http.Unauthorized()), HttpStatusCode.Unauthorized);

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            
            var token = context.HttpContext.Request.Query["code"].FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                context.Result = Unauthorized();
                return;
            }

            if (_tokenService.IsExpired(token))
            {
                context.Result = Unauthorized();
                return;
            }

            await next();
        }
    }
}
