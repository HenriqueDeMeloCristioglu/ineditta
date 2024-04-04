using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ineditta.BuildingBlocks.Core.Web.API.Binders.EnumAsString
{
    public class EnumStringModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.IsEnum ? new EnumStringModelBinder() : (IModelBinder?)null;
        }
    }
}
