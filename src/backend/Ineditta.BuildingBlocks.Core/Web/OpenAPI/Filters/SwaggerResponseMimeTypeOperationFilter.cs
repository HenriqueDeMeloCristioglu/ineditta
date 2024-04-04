using System.Globalization;
using System.Net;

using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ineditta.BuildingBlocks.Core.Web.OpenAPI.Filters
{
#pragma warning disable IDE0066 // Convert switch statement to expression
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    public class SwaggerResponseMimeTypeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var attrs = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<SwaggerResponseMimeTypeAttribute>()
                .ToList();

            var declType = context.MethodInfo.DeclaringType;
            while (declType != null)
            {
                attrs.AddRange(declType
                    .GetCustomAttributes(true)
                    .OfType<SwaggerResponseMimeTypeAttribute>());

                declType = declType.DeclaringType;
            }

            if (attrs.Any())
            {
                foreach (var attr in attrs)
                {
                    HttpStatusCode statusCode = (HttpStatusCode)attr.StatusCode;
                    string statusString = attr.StatusCode.ToString(CultureInfo.CurrentCulture);


                    if (!operation.Responses.TryGetValue(statusString, out OpenApiResponse response))
                    {
                        response = new OpenApiResponse();
                        operation.Responses.Add(statusString, response);
                    }

                    if (!string.IsNullOrEmpty(attr.Description))
                        response.Description = attr.Description;
                    else if (string.IsNullOrEmpty(response.Description))
                        response.Description = statusCode.ToString();

                    response.Content ??= new Dictionary<string, OpenApiMediaType>();

                    var openApiMediaType = new OpenApiMediaType();

#pragma warning disable S3358 // Ternary operators should not be nested
                    string? swaggerDataType =
                        IsNumericType(attr.Type) ? "number"
                        : IsStringType(attr.Type) ? "string"
                        : IsBooleanType(attr.Type) ? "boolean"
                        : null;
#pragma warning restore S3358 // Ternary operators should not be nested

                    if (swaggerDataType == null)
                    {
                        // this is not a native type, try to register it in the repository
                        if (!context.SchemaRepository.TryLookupByType(attr.Type, out var schema))
                        {
                            schema = context.SchemaGenerator.GenerateSchema(attr.Type, context.SchemaRepository);

                            if (schema == null)
                                throw new InvalidOperationException($"Failed to register swagger schema type '{attr.Type.Name}'");
                        }

                        openApiMediaType.Schema = schema;
                    }
                    else
                    {
                        openApiMediaType.Schema = new OpenApiSchema
                        {
                            Type = swaggerDataType
                        };
                    }

                    if (attr.MediaTypes != null)
                    {
                        foreach (string mediaType in attr.MediaTypes)
                        {
                            if (!response.Content.Any(ct => ct.Key == mediaType))
                            {
                                response.Content.Add(mediaType, openApiMediaType);
                            }
                        }   
                    }
                }
            }
        }

        private static bool IsNumericType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsStringType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsBooleanType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SwaggerResponseMimeTypeAttribute : Attribute
    {
        public int StatusCode { get; set; }
        public Type Type { get; set; }
        public string[]? MediaTypes { get; set; }
        public string? Description { get; set; }

        public SwaggerResponseMimeTypeAttribute(int statusCode, Type type, params string[] mediaTypes)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (!mediaTypes?.Any() ?? true)
            {
                throw new ArgumentNullException(nameof(mediaTypes));
            }

            StatusCode = statusCode;
            Type = type;
            MediaTypes = mediaTypes;
        }
    }

#pragma warning restore IDE0066 // Convert switch statement to expression
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
}
