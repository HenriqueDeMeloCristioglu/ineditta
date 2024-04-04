using System.Net;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Ineditta.BuildingBlocks.Core.Web.API.Actions
{
    public sealed class EnvelopeResult : IActionResult
    {
        private readonly Envelope _envelope;
        private readonly int _statusCode;

        public EnvelopeResult(Envelope envelope, HttpStatusCode statusCode)
        {
            _statusCode = (int)statusCode;
            _envelope = envelope;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_envelope)
            {
                StatusCode = _statusCode
            };

            return objectResult.ExecuteResultAsync(context);
        }
    }
}
