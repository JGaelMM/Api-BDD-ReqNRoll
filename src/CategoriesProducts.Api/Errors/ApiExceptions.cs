namespace CategoriesProducts.Api.Errors;

/// <summary>
/// Error de negocio que se traduce a una respuesta HTTP con ProblemDetails.
/// </summary>
public abstract class ApiException(int statusCode, string title, string message) : Exception(message)
{
    public int StatusCode { get; } = statusCode;

    public string Title { get; } = title;
}

/// <summary>Los datos enviados no cumplen las reglas (400).</summary>
public class ValidationException(string message)
    : ApiException(StatusCodes.Status400BadRequest, "Datos inválidos", message);

/// <summary>El recurso solicitado no existe (404).</summary>
public class NotFoundException(string message)
    : ApiException(StatusCodes.Status404NotFound, "No encontrado", message);

/// <summary>La operación choca con el estado actual de los datos (409).</summary>
public class ConflictException(string message)
    : ApiException(StatusCodes.Status409Conflict, "Conflicto", message);
