using BuildingBlocks.SharedKernel.Errors;
using BuildingBlocks.SharedKernel.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Payment.Presentation.ExceptionHandlers;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ErrorCode error;

        if (exception is AppException appException)
        {
            error = appException.Error;
        }
        else
        {
            logger.LogError(
                exception,
                "Unhandled exception occurred"
            );

            error = SystemErrors.InternalError;
        }

        var statusCode = exception is AppException
            ? MapStatusCode(error.Type)
            : StatusCodes.Status500InternalServerError;

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(
            new
            {
                code = error.Code,
                message = error.Message
            },
            cancellationToken
        );

        return true;
    }

    private static int MapStatusCode(ErrorType type) =>
        type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
}

