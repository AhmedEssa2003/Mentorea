using Microsoft.AspNetCore.Diagnostics;

namespace Mentorea.Errors
{
    public class GlobbalExceptionHandler(ILogger<GlobbalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger<GlobbalExceptionHandler> _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Something went wrong {Message}",exception.Message);
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.2.1"
            };
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails);
            return true;
        }
    }
}
