using System.Net;
using Ineditta.BuildingBlocks.Core.Web.API.Actions;
using Ineditta.BuildingBlocks.Core.Web.API.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using Microsoft.AspNetCore.Mvc;
using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.BuildingBlocks.Core.Web.API.Validators
{
    public static class ModelStateValidator
    {
        public static IActionResult ValidateModelState(ActionContext context)
        {
            (string fieldName, ModelStateEntry? entry) = context.ModelState.First(x => x.Value?.Errors?.Count > 0);

            var errors = entry!.Errors!.Select(error => EnvolopeError.Create(Error.Deserialize(error.ErrorMessage), fieldName));

            Envelope envelope = Envelope.Error(errors);

            var envelopeResult = new EnvelopeResult(envelope, HttpStatusCode.BadRequest);

            return envelopeResult;
        }
    }
}
