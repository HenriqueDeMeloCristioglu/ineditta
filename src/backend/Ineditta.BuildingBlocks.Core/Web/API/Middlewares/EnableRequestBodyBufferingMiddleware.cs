using Microsoft.AspNetCore.Http;

namespace Ineditta.BuildingBlocks.Core.Web.API.Middlewares
{
    public class EnableRequestBodyBufferingMiddleware
    {
        private readonly RequestDelegate _next;

        public EnableRequestBodyBufferingMiddleware(RequestDelegate next) =>
            _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Body.CanSeek)
            {
                context.Request.EnableBuffering();
            }

            context.Request.Body.Position = 0;

            await _next(context);
        }
    }
}
