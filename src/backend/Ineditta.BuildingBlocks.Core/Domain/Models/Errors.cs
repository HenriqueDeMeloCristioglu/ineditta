using Microsoft.Net.Http.Headers;

namespace Ineditta.BuildingBlocks.Core.Domain.Models
{
    public static class Errors
    {
        public static class General
        {
            public static Error NotFound(long? id = null)
            {
                string forId = id == null ? "" : $" for Id '{id}'";
                return new Error("record.not.found", $"Record not found{forId}");
            }

            public static Error ValueIsInvalid() =>
                new("value.is.invalid", "Value is invalid");

            public static Error ValueIsRequired() =>
                new("value.is.required", "Value is required");

            public static Error InvalidLength(string? name = null)
            {
                string label = name == null ? " " : " " + name + " ";
                return new Error("invalid.string.length", $"Invalid{label}length");
            }

            public static Error CollectionIsTooSmall(int min, int current)
            {
                return new Error(
                    "collection.is.too.small",
                    $"The collection must contain {min} items or more. It contains {current} items.");
            }

            public static Error CollectionIsTooLarge(int max, int current)
            {
                return new Error(
                    "collection.is.too.large",
                    $"The collection must contain {max} items or more. It contains {current} items.");
            }

            public static Error InternalServerError(string message)
            {
                return new Error("internal.server.error", message);
            }

            public static Error Business(string message)
            {
                return new Error("business", message);
            }
        }

        public static class Http
        {
            public static Error NotFound()
            {
                return new Error("http.not.found", $"Resource not found");
            }
            public static Error InvalidHeader(string headerName)
            {
                return new Error("http.invalid.header", $"Invalid header {headerName}");
            }

            public static Error InvalidPayload()
            {
                return new Error("http.invalid.payload", "Invalid payload");
            }

            public static Error BadRequest(string error)
            {
                return new Error("http.bad.request", error);
            }

            public static Error EmptyRequestBody()
            {
                return new Error("http.empty.request.body", "Missing or empty request body");
            }

            public static Error NotAcceptable()
            {
                return new Error("http.not.acceptable", "Not acceptable");
            }

            public static Error Unauthorized()
            {
                return new Error("http.unauthorized", "Unauthorized");
            }

            internal static Error Duplicated()
            {
                return new Error("http.duplicated", "Duplicated");
            }

            public static Error TooManyRequests()
            {
                return new Error("http.too.many.requests", $"Too many requests");
            }

            public static Error ErrorCode()
            {
                return new Error("http.error.code", $"Https status code between 500 and 599");
            }

            public static Error Timeout(string error)
            {
                return new Error("http.timeout", error);
            }
        }

        public static class Html
        {
            public static Error TemplateRender()
            {
                return new Error("html.template.render", $"Failure to bind template");
            }
        }
    }
}
