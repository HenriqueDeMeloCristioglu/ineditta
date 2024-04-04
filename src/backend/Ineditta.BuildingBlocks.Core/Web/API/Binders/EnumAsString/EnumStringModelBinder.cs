using System.Reflection;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString
{
    public class EnumStringModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var isBindAsString = bindingContext.ModelType.GetCustomAttribute<BindAsStringAttribute>() != null;

            if (!isBindAsString)
            {
                return Task.CompletedTask;
            }

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            bindingContext.Result = Enum.TryParse(bindingContext.ModelType, value, true, out var result)
                ? ModelBindingResult.Success(result)
                : ModelBindingResult.Failed();

            return Task.CompletedTask;
        }
    }
}
