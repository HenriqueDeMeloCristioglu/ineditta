using System.Net;

using Ineditta.BuildingBlocks.Core.Domain.Models;
using Ineditta.BuildingBlocks.Core.Web.API.Actions;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;

using Microsoft.AspNetCore.Mvc.Filters;

namespace Ineditta.Integration.Email.Providers.Aws.Filters
{
    public class AwsValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (Amazon.SimpleNotificationService.Util.Message.ParseMessage(context.HttpContext.Items["RequestBody"]?.ToString()).IsMessageSignatureValid())
            {
                await next();
                return;
            }

            context.Result = new EnvelopeResult(Envelope.Error(Errors.Http.Unauthorized()), HttpStatusCode.Forbidden);
        }
    }
}
