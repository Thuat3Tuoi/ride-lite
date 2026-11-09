using MatchingPricing.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing.Presentation.ExceptionHandlers;

// Chi dua thong diep validation ra ngoai, loi noi bo duoc giu trong log server
public sealed class QuoteExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        bool validation = exception is QuoteValidationException;
        int status = validation ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = validation ? "Invalid quote request" : "An internal error occurred",
            Detail = validation ? exception.Message : "Unable to calculate the quote",
            Instance = context.Request.Path
        }, options: null, contentType: "application/problem+json", cancellationToken: cancellationToken);
        return true;
    }
}
