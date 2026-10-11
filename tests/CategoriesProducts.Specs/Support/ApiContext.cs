using System.Text.Json;

namespace CategoriesProducts.Specs.Support;

/// <summary>
/// Estado compartido entre los steps de un mismo escenario.
/// ReqNRoll crea una instancia nueva por escenario y la inyecta por constructor.
/// </summary>
public class ApiContext
{
    private HttpResponseMessage? _lastResponse;
    private string? _lastBody;

    public HttpResponseMessage? LastResponse
    {
        get => _lastResponse;
        set
        {
            _lastResponse = value;
            _lastBody = null;
        }
    }

    public HttpResponseMessage Response =>
        LastResponse ?? throw new InvalidOperationException("Ningún step ha hecho una petición a la API todavía.");

    /// <summary>Ids de las categorías creadas en el escenario, por nombre.</summary>
    public Dictionary<string, int> CategoryIds { get; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Deserializa el cuerpo de la última respuesta. El contenido HTTP solo se puede leer una vez,
    /// así que se guarda para que varios steps "Entonces" puedan revisarlo.
    /// </summary>
    public async Task<T> ReadBodyAsync<T>()
    {
        _lastBody ??= await Response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(_lastBody, JsonSerializerOptions.Web)
            ?? throw new InvalidOperationException($"La respuesta no contiene un {typeof(T).Name}.");
    }
}
