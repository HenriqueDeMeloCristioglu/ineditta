using FluentValidation;

using Ineditta.BuildingBlocks.Core.Web.API.Validators;

using MediatR;

namespace Ineditta.BuildingBlocks.Core.Web.API.Pipelines
{
    public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?>
    where TRequest : class, IRequest<TResponse?>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly RequestStateValidator _requestValidatorState;
        public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators, RequestStateValidator requestValidatorState)
        {
            _validators = validators ?? throw new ArgumentNullException(nameof(validators));
            _requestValidatorState = requestValidatorState ?? throw new ArgumentNullException(nameof(requestValidatorState));
        }

        public async Task<TResponse?> Handle(TRequest request, RequestHandlerDelegate<TResponse?> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next();
            }

            var context = new ValidationContext<TRequest>(request);

            var errors = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .ToList();

            if (errors.Any())
            {
                _requestValidatorState.Add(errors);

                return default;
            }

            return await next();
        }
    }
}
