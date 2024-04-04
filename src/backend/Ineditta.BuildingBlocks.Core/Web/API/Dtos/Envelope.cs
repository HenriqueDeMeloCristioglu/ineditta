using System.Diagnostics.CodeAnalysis;

using Ineditta.BuildingBlocks.Core.Domain.Models;

namespace Ineditta.BuildingBlocks.Core.Web.API.Dtos
{
    public class Envelope
    {
        private Envelope(object? result, IEnumerable<EnvolopeError>? errors)
        {
            Result = result;
            Errors = errors;
            TimeGenerated = DateTime.UtcNow;
        }

        public static Envelope Ok(object? result)
        {
            return new Envelope(result, null);
        }

        public static Envelope Error([NotNull] IEnumerable<EnvolopeError> errors)
        {
            return new Envelope(default, errors);
        }

        public static Envelope Error([NotNull] Error error)
        {
            return new Envelope(default, new List<EnvolopeError> { error });
        }

        public static Envelope Error([NotNull] IEnumerable<Error> errors)
        {
            return new Envelope(default, errors.Select(error => (EnvolopeError)error));
        }

        public object? Result { get; init; }
        public IEnumerable<EnvolopeError>? Errors { get; init; }
        public DateTime TimeGenerated { get; init; }
    }
}