using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Ineditta.BuildingBlocks.Core.Web.API.Attributes;

namespace Ineditta.BuildingBlocks.Core.Web.API.Middlewares
{
    public class ReadRequestBodyMiddleware
    {
        private readonly RequestDelegate _next;
        public ReadRequestBodyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // check for the attribute and skip everything else if it exists
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ReadRequestBodyAttribute>();
            if (attribute == null)
            {
                await _next(context);
                return;
            }

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(context.Request.Body);
            var requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            context.Items["RequestBody"] = requestBody;

            await _next(context);
        }
    }
}
