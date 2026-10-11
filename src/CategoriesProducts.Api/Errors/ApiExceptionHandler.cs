using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesProducts.Api.Errors;

/// <summary>
/// Convierte las <see cref="ApiException"/> en respuestas ProblemDetails con su código HTTP.
/// Cualquier otra excepción la deja pasar para que se responda 500 sin exponer detalles internos.
/// </summary>
public class ApiExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ApiException apiException)
        {
            return false;
        }

        httpContext.Response.StatusCode = apiException.StatusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = apiException.StatusCode,
                Title = apiException.Title,
                Detail = apiException.Message
            }
        });
    }
}
