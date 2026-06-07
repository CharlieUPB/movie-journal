using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MovieJournal.Application.Exceptions;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Web.ExceptionHandling;

public class ApplicationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is UnauthorizedAccessException unauthorizedAccessException)
        {
            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                unauthorizedAccessException.Message,
                cancellationToken);

            return true;
        }

        if (exception is DomainException domainException)
        {
            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Domain validation error",
                domainException.Message,
                cancellationToken);

            return true;
        }

        if (exception is UseCaseException useCaseException)
        {
            await WriteProblemDetailsAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                "Use case error",
                useCaseException.Message,
                cancellationToken);

            return true;
        }

        return false;
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            },
            cancellationToken);
    }
}
