using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Net;
using System.Text.Json;

namespace Ineditta.BuildingBlocks.Core.Web.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger logger, IHostEnvironment env)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
#pragma warning disable CA1848 // Use the LoggerMessage delegates
                _logger.LogError("Não foi possível processar a requisição: {Exception}", exception);
#pragma warning restore CA1848 // Use the LoggerMessage delegates

                await HandleExceptionAsync(httpContext, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string errorMessage = _env.IsProduction() || _env.IsEnvironment("Homologacao") ? "Internal server error" : "Exception: " + exception.Message;

            var error = Errors.General.InternalServerError(errorMessage);

            var envelope = Envelope.Error(error);

            string result = JsonSerializer.Serialize(envelope);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await context.Response.WriteAsync(result);
        }
    }
}
