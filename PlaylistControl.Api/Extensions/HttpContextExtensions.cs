using FluentValidation;
using FluentValidation.Results;

namespace PlaylistControl.Api.Extensions
{
    /// <summary>
    /// Helpers for extracting the acting user from the incoming request.
    /// </summary>
    public static class HttpContextExtensions
    {
        private const string RequesterHeaderName = "X-User-Id";

        /// <summary>
        /// Reads the acting user's identifier from the <c>X-User-Id</c> header.
        /// </summary>
        /// <param name="context">Current HTTP context</param>
        /// <returns>The parsed requester identifier</returns>
        /// <exception cref="ValidationException">
        /// Thrown when the header is missing, empty, or not a valid GUID.
        /// </exception>
        public static Guid GetRequesterId(this HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(RequesterHeaderName, out var raw) ||
                string.IsNullOrWhiteSpace(raw))
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(RequesterHeaderName, $"Header '{RequesterHeaderName}' is required.")
                });
            }

            if (!Guid.TryParse(raw, out var requesterId) || requesterId == Guid.Empty)
            {
                throw new ValidationException(new[]
                {
                    new ValidationFailure(RequesterHeaderName, $"Header '{RequesterHeaderName}' must be a non-empty GUID.")
                });
            }

            return requesterId;
        }
    }
}