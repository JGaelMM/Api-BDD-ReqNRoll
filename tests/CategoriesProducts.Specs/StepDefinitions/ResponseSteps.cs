using System.Net;
using CategoriesProducts.Specs.Support;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesProducts.Specs.StepDefinitions;

/// <summary>
/// Steps de resultado comunes a categorías y productos.
/// </summary>
[Binding]
public class ResponseSteps(ApiContext context)
{
    [Then("la solicitud es rechazada por datos inválidos")]
    public void ThenRejectedAsInvalid() => Assert.Equal(HttpStatusCode.BadRequest, context.Response.StatusCode);

    [Then("la solicitud es rechazada por conflicto")]
    public void ThenRejectedAsConflict() => Assert.Equal(HttpStatusCode.Conflict, context.Response.StatusCode);

    [Then("el mensaje de error es {string}")]
    public async Task ThenTheErrorMessageIs(string message)
    {
        var problem = await context.ReadBodyAsync<ProblemDetails>();

        Assert.Equal(message, problem.Detail);
    }
}
