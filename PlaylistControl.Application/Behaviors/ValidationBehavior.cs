using FluentValidation;
using MediatR;

namespace PlaylistControl.Application.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that runs all registered FluentValidation validators
    /// for a request before the request reaches its handler. Throws
    /// <see cref="ValidationException"/> when any validator reports failures.
    /// </summary>
    /// <typeparam name="TRequest">The request type being handled</typeparam>
    /// <typeparam name="TResponse">The response type produced by the handler</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="validators">Validators registered for <typeparamref name="TRequest"/></param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Runs all validators for the request, throwing <see cref="ValidationException"/> if any produce failures, otherwise invoking the next delegate in the pipeline.
        /// </summary>
        /// <param name="request">The request being handled</param>
        /// <param name="next">The next delegate in the MediatR pipeline</param>
        /// <param name="cancellationToken">Token to cancel the operation</param>
        /// <returns>The response produced by the handler</returns>
        /// <exception cref="ValidationException">Thrown when one or more validators report failures</exception>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var results = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = results
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count != 0)
                throw new ValidationException(failures);

            return await next();
        }
    }
}