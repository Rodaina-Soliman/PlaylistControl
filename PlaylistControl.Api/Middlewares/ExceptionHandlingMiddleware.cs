using System.Text.Json;
using FluentValidation;
using PlaylistControl.Domain.Exceptions;

namespace PlaylistControl.Api.Middleware
{
    /// <summary>
    /// Translates unhandled exceptions into consistent HTTP responses.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">Next middleware in the pipeline</param>
        /// <param name="logger">Logger</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">Current HTTP context</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation failure.");
                await WriteResponseAsync(context, StatusCodes.Status400BadRequest, new
                {
                    title = "One or more validation errors occurred.",
                    status = StatusCodes.Status400BadRequest,
                    errors = ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                });
            }
            catch (PlaylistNotFoundException ex)
            {
                await WriteNotFoundAsync(context, ex);
            }
            catch (UserNotFoundException ex)
            {
                await WriteNotFoundAsync(context, ex);
            }
            catch (SongNotFoundException ex)
            {
                await WriteNotFoundAsync(context, ex);
            }
            catch (NotPlaylistOwnerException ex)
            {
                await WriteForbiddenAsync(context, ex);
            }
            catch (PrivatePlaylistAccessException ex)
            {
                await WriteForbiddenAsync(context, ex);
            }
            catch (RedundantOperationException ex)
            {
                await WriteBadRequestAsync(context, ex);
            }
            catch (DomainException ex)
            {
                await WriteBadRequestAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception.");
                await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, new
                {
                    title = "An unexpected error occurred.",
                    status = StatusCodes.Status500InternalServerError
                });
            }
        }

        private static Task WriteNotFoundAsync(HttpContext context, Exception ex)
            => WriteResponseAsync(context, StatusCodes.Status404NotFound, new
            {
                title = ex.Message,
                status = StatusCodes.Status404NotFound
            });

        private static Task WriteForbiddenAsync(HttpContext context, Exception ex)
            => WriteResponseAsync(context, StatusCodes.Status403Forbidden, new
            {
                title = ex.Message,
                status = StatusCodes.Status403Forbidden
            });

        private static Task WriteBadRequestAsync(HttpContext context, Exception ex)
            => WriteResponseAsync(context, StatusCodes.Status400BadRequest, new
            {
                title = ex.Message,
                status = StatusCodes.Status400BadRequest
            });

        private static async Task WriteResponseAsync(HttpContext context, int statusCode, object payload)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}