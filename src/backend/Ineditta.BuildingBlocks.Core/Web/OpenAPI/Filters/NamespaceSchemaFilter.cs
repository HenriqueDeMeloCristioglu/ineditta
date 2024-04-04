using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters
{
    public class NamespaceSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema is null)
            {
                throw new System.ArgumentNullException(nameof(schema));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            schema.Title = context.Type.Name;
        }
    }
}
