using System.Net;
using System.Net.Http.Json;
using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Specs.Support;

namespace CategoriesProducts.Specs.StepDefinitions;

[Binding]
public class CategorySteps(HttpClient client, ApiContext context)
{
    private const string CategoriesUrl = "/api/categories";

    // ---------- Dado ----------

    [Given("que existe una categoría con nombre {string}")]
    public Task GivenACategoryExists(string name) => CreateExistingCategory(name, null);

    private async Task CreateExistingCategory(string name, string? description)
    {
        var response = await client.PostAsJsonAsync(CategoriesUrl, new CategoryRequest(name, description));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var category = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        context.CategoryIds[category!.Name] = category.Id;
    }

    // ---------- Cuando ----------

    [When("creo una categoría con nombre {string} y descripción {string}")]
    public Task WhenICreateACategory(string name, string description) => CreateCategory(name, description);

    [When("creo una categoría con nombre {string} sin descripción")]
    public Task WhenICreateACategoryWithoutDescription(string name) => CreateCategory(name, null);

    [When("creo una categoría sin nombre")]
    public Task WhenICreateACategoryWithoutName() => CreateCategory(null, null);

    [When("creo una categoría con un nombre de {int} caracteres")]
    public Task WhenICreateACategoryWithNameLength(int length) => CreateCategory(new string('a', length), null);

    [When("creo una categoría con nombre {string} y una descripción de {int} caracteres")]
    public Task WhenICreateACategoryWithDescriptionLength(string name, int length) =>
        CreateCategory(name, new string('a', length));

    private async Task CreateCategory(string? name, string? description) =>
        context.LastResponse = await client.PostAsJsonAsync(CategoriesUrl, new CategoryRequest(name, description));

    // ---------- Entonces ----------

    [Then("la categoría se crea correctamente")]
    public async Task ThenTheCategoryIsCreated()
    {
        Assert.Equal(HttpStatusCode.Created, context.Response.StatusCode);

        var category = await ReadCategory();
        Assert.Equal($"{CategoriesUrl}/{category.Id}", context.Response.Headers.Location?.OriginalString);
    }

    [Then("la categoría creada tiene el nombre {string}")]
    public async Task ThenTheCreatedCategoryHasName(string name) => Assert.Equal(name, (await ReadCategory()).Name);

    [Then("la categoría creada tiene la descripción {string}")]
    public async Task ThenTheCreatedCategoryHasDescription(string description) =>
        Assert.Equal(description, (await ReadCategory()).Description);

    [Then("la categoría creada no tiene descripción")]
    public async Task ThenTheCreatedCategoryHasNoDescription() => Assert.Null((await ReadCategory()).Description);

    private Task<CategoryResponse> ReadCategory() => context.ReadBodyAsync<CategoryResponse>();
}
