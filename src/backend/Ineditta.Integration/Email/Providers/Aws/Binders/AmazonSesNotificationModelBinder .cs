using Ineditta.Integration.Email.Providers.Aws.Models;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using Newtonsoft.Json;

namespace Ineditta.Integration.Email.Providers.Aws.Binders
{
    public class AmazonSesNotificationModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            try
            {
                // Read the request body
                using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
                var requestBody = await reader.ReadToEndAsync();

                bindingContext.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);

                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                // Deserialize the JSON body into the model
                var model = JsonConvert.DeserializeObject<AmazonSesNotificationRequest>(requestBody, settings);

                // Set the model to the binding context
                bindingContext.Result = ModelBindingResult.Success(model);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
            }
        }
    }
}
