using System.Net;
using System.Net.Http.Json;
using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Specs.Support;

namespace CategoriesProducts.Specs.StepDefinitions;

[Binding]
public class CategorySteps(HttpClient client, ApiContext context)
{
    private const string CategoriesUrl = "/api/categories";
    private const int NonExistentId = int.MaxValue;

    // ---------- Dado ----------

    [Given("que existe una categoría con nombre {string}")]
    public Task GivenACategoryExists(string name) => CreateExistingCategory(name, null);

    [Given("que existe una categoría con nombre {string} y descripción {string}")]
    public Task GivenACategoryWithDescriptionExists(string name, string description) =>
        CreateExistingCategory(name, description);

    [Given("que existen las categorías:")]
    public async Task GivenTheseCategoriesExist(DataTable table)
    {
        foreach (var row in table.Rows)
        {
            await CreateExistingCategory(row["nombre"], EmptyAsNull(row["descripción"]));
        }
    }

    [Given("que existen {int} categorías")]
    public async Task GivenManyCategoriesExist(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            await CreateExistingCategory($"Categoría {i:D2}", null);
        }
    }

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

    [When("consulto la lista de categorías")]
    public async Task WhenIListCategories() => context.LastResponse = await client.GetAsync(CategoriesUrl);

    [When("consulto la página {int} de la lista de categorías con {int} categorías por página")]
    public async Task WhenIListACategoryPage(int page, int pageSize) =>
        context.LastResponse = await client.GetAsync($"{CategoriesUrl}?page={page}&pageSize={pageSize}");

    [When("consulto el detalle de la categoría {string}")]
    public async Task WhenIGetTheCategory(string name) =>
        context.LastResponse = await client.GetAsync($"{CategoriesUrl}/{context.CategoryIds[name]}");

    [When("consulto el detalle de una categoría que no existe")]
    public async Task WhenIGetANonExistentCategory() =>
        context.LastResponse = await client.GetAsync($"{CategoriesUrl}/{NonExistentId}");

    // ---------- Entonces ----------

    [Then("la categoría se crea correctamente")]
    public async Task ThenTheCategoryIsCreated()
    {
        Assert.Equal(HttpStatusCode.Created, context.Response.StatusCode);

        var category = await ReadCategory();
        Assert.EndsWith($"{CategoriesUrl}/{category.Id}", context.Response.Headers.Location?.OriginalString);
    }

    [Then("la categoría creada tiene el nombre {string}")]
    public async Task ThenTheCreatedCategoryHasName(string name) => Assert.Equal(name, (await ReadCategory()).Name);

    [Then("la categoría creada tiene la descripción {string}")]
    public async Task ThenTheCreatedCategoryHasDescription(string description) =>
        Assert.Equal(description, (await ReadCategory()).Description);

    [Then("la categoría creada no tiene descripción")]
    public async Task ThenTheCreatedCategoryHasNoDescription() => Assert.Null((await ReadCategory()).Description);

    [Then("la categoría se encuentra")]
    public void ThenTheCategoryIsFound() => Assert.Equal(HttpStatusCode.OK, context.Response.StatusCode);

    [Then("la categoría no se encuentra")]
    public void ThenTheCategoryIsNotFound() => Assert.Equal(HttpStatusCode.NotFound, context.Response.StatusCode);

    [Then("la categoría consultada tiene el nombre {string}")]
    public Task ThenTheCategoryHasName(string name) => ThenTheCreatedCategoryHasName(name);

    [Then("la categoría consultada tiene la descripción {string}")]
    public Task ThenTheCategoryHasDescription(string description) => ThenTheCreatedCategoryHasDescription(description);

    [Then("la lista contiene {int} categorías de un total de {int}")]
    public async Task ThenTheListContains(int count, int totalCount)
    {
        Assert.Equal(HttpStatusCode.OK, context.Response.StatusCode);

        var page = await ReadCategoryPage();
        Assert.Equal(count, page.Items.Count);
        Assert.Equal(totalCount, page.TotalCount);
    }

    [Then("la lista corresponde a la página {int} con {int} categorías por página")]
    public async Task ThenTheListIsPage(int pageNumber, int pageSize)
    {
        var page = await ReadCategoryPage();
        Assert.Equal(pageNumber, page.Page);
        Assert.Equal(pageSize, page.PageSize);
    }

    [Then("la lista muestra las categorías en este orden:")]
    public async Task ThenTheListShowsInOrder(DataTable table)
    {
        var expected = table.Rows.Select(row => (row["nombre"], EmptyAsNull(row["descripción"])));

        var page = await ReadCategoryPage();
        Assert.Equal(expected, page.Items.Select(c => (c.Name, c.Description)));
    }

    private Task<CategoryResponse> ReadCategory() => context.ReadBodyAsync<CategoryResponse>();

    private Task<PagedResponse<CategoryResponse>> ReadCategoryPage() =>
        context.ReadBodyAsync<PagedResponse<CategoryResponse>>();

    // Una celda vacía en una tabla de Gherkin significa "sin valor".
    private static string? EmptyAsNull(string value) => value.Length == 0 ? null : value;
}
