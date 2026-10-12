using System.Net;
using System.Net.Http.Json;
using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Specs.Support;
using static CategoriesProducts.Specs.Support.StepValues;

namespace CategoriesProducts.Specs.StepDefinitions;

[Binding]
public class ProductSteps(HttpClient client, ApiContext context)
{
    private const string ProductsUrl = "/api/products";

    private int _deletedProductId;

    // ---------- Dado ----------

    [Given("que existe un producto con nombre {string} en la categoría {string}")]
    public Task GivenAProductExists(string name, string category) => CreateExistingProduct(name, null, category);

    [Given("que existe un producto con nombre {string} y descripción {string} en la categoría {string}")]
    public Task GivenAProductWithDescriptionExists(string name, string description, string category) =>
        CreateExistingProduct(name, description, category);

    [Given("que existen los productos:")]
    public async Task GivenTheseProductsExist(DataTable table)
    {
        foreach (var row in table.Rows)
        {
            await CreateExistingProduct(row["nombre"], EmptyAsNull(row["descripción"]), row["categoría"]);
        }
    }

    [Given("que existen {int} productos en la categoría {string}")]
    public async Task GivenManyProductsExist(int count, string category)
    {
        for (var i = 1; i <= count; i++)
        {
            await CreateExistingProduct($"Producto {i:D2}", null, category);
        }
    }

    private async Task CreateExistingProduct(string name, string? description, string category)
    {
        var request = new ProductRequest(name, description, context.CategoryIds[category]);
        var response = await client.PostAsJsonAsync(ProductsUrl, request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductResponse>();
        context.ProductIds[product!.Name] = product.Id;
    }

    // ---------- Cuando ----------

    [When("creo un producto con nombre {string}, descripción {string} y categoría {string}")]
    public Task WhenICreateAProduct(string name, string description, string category) =>
        CreateProduct(name, description, context.CategoryIds[category]);

    [When("creo un producto con nombre {string} sin descripción en la categoría {string}")]
    public Task WhenICreateAProductWithoutDescription(string name, string category) =>
        CreateProduct(name, null, context.CategoryIds[category]);

    [When("creo un producto sin nombre en la categoría {string}")]
    public Task WhenICreateAProductWithoutName(string category) =>
        CreateProduct(null, null, context.CategoryIds[category]);

    [When("creo un producto con un nombre de {int} caracteres en la categoría {string}")]
    public Task WhenICreateAProductWithNameLength(int length, string category) =>
        CreateProduct(new string('a', length), null, context.CategoryIds[category]);

    [When("creo un producto con nombre {string} y una descripción de {int} caracteres en la categoría {string}")]
    public Task WhenICreateAProductWithDescriptionLength(string name, int length, string category) =>
        CreateProduct(name, new string('a', length), context.CategoryIds[category]);

    [When("creo un producto con nombre {string} sin categoría")]
    public Task WhenICreateAProductWithoutCategory(string name) => CreateProduct(name, null, null);

    [When("creo un producto con nombre {string} en una categoría que no existe")]
    public Task WhenICreateAProductInANonExistentCategory(string name) => CreateProduct(name, null, NonExistentId);

    private async Task CreateProduct(string? name, string? description, int? categoryId) =>
        context.LastResponse = await client.PostAsJsonAsync(ProductsUrl, new ProductRequest(name, description, categoryId));

    [When("edito el producto {string} con nombre {string}, descripción {string} y categoría {string}")]
    public Task WhenIUpdateAProduct(string currentName, string name, string description, string category) =>
        UpdateProduct(context.ProductIds[currentName], name, description, context.CategoryIds[category]);

    [When("edito el producto {string} con nombre {string} sin descripción en la categoría {string}")]
    public Task WhenIUpdateAProductWithoutDescription(string currentName, string name, string category) =>
        UpdateProduct(context.ProductIds[currentName], name, null, context.CategoryIds[category]);

    [When("edito el producto {string} sin nombre en la categoría {string}")]
    public Task WhenIUpdateAProductWithoutName(string currentName, string category) =>
        UpdateProduct(context.ProductIds[currentName], null, null, context.CategoryIds[category]);

    [When("edito el producto {string} con un nombre de {int} caracteres en la categoría {string}")]
    public Task WhenIUpdateAProductWithNameLength(string currentName, int length, string category) =>
        UpdateProduct(context.ProductIds[currentName], new string('a', length), null, context.CategoryIds[category]);

    [When("edito el producto {string} con nombre {string} y una descripción de {int} caracteres en la categoría {string}")]
    public Task WhenIUpdateAProductWithDescriptionLength(string currentName, string name, int length, string category) =>
        UpdateProduct(context.ProductIds[currentName], name, new string('a', length), context.CategoryIds[category]);

    [When("edito el producto {string} con nombre {string} sin categoría")]
    public Task WhenIUpdateAProductWithoutCategory(string currentName, string name) =>
        UpdateProduct(context.ProductIds[currentName], name, null, null);

    [When("edito el producto {string} con nombre {string} en una categoría que no existe")]
    public Task WhenIUpdateAProductToANonExistentCategory(string currentName, string name) =>
        UpdateProduct(context.ProductIds[currentName], name, null, NonExistentId);

    [When("edito un producto que no existe con nombre {string} en la categoría {string}")]
    public Task WhenIUpdateANonExistentProduct(string name, string category) =>
        UpdateProduct(NonExistentId, name, null, context.CategoryIds[category]);

    private async Task UpdateProduct(int id, string? name, string? description, int? categoryId)
    {
        context.LastResponse = await client.PutAsJsonAsync($"{ProductsUrl}/{id}", new ProductRequest(name, description, categoryId));

        // Si cambió de nombre, los siguientes steps lo buscan por el nombre nuevo.
        if (context.LastResponse.IsSuccessStatusCode)
        {
            var product = await ReadProduct();
            context.ProductIds[product.Name] = product.Id;
        }
    }

    [When("elimino el producto {string}")]
    public Task WhenIDeleteTheProduct(string name) => DeleteProduct(context.ProductIds[name]);

    [When("elimino un producto que no existe")]
    public Task WhenIDeleteANonExistentProduct() => DeleteProduct(NonExistentId);

    private async Task DeleteProduct(int id)
    {
        _deletedProductId = id;
        context.LastResponse = await client.DeleteAsync($"{ProductsUrl}/{id}");
    }

    [When("consulto la lista de productos")]
    public async Task WhenIListProducts() => context.LastResponse = await client.GetAsync(ProductsUrl);

    [When("consulto la página {int} de la lista de productos con {int} productos por página")]
    public async Task WhenIListAProductPage(int page, int pageSize) =>
        context.LastResponse = await client.GetAsync($"{ProductsUrl}?page={page}&pageSize={pageSize}");

    [When("consulto el detalle del producto {string}")]
    public async Task WhenIGetTheProduct(string name) =>
        context.LastResponse = await client.GetAsync($"{ProductsUrl}/{context.ProductIds[name]}");

    [When("consulto el detalle de un producto que no existe")]
    public async Task WhenIGetANonExistentProduct() =>
        context.LastResponse = await client.GetAsync($"{ProductsUrl}/{NonExistentId}");

    // ---------- Entonces ----------

    [Then("el producto se crea correctamente")]
    public async Task ThenTheProductIsCreated()
    {
        Assert.Equal(HttpStatusCode.Created, context.Response.StatusCode);

        var product = await ReadProduct();
        Assert.EndsWith($"{ProductsUrl}/{product.Id}", context.Response.Headers.Location?.OriginalString);
    }

    [Then("el producto creado tiene el nombre {string}")]
    public async Task ThenTheProductHasName(string name) => Assert.Equal(name, (await ReadProduct()).Name);

    [Then("el producto creado tiene la descripción {string}")]
    public async Task ThenTheProductHasDescription(string description) =>
        Assert.Equal(description, (await ReadProduct()).Description);

    [Then("el producto creado no tiene descripción")]
    public async Task ThenTheProductHasNoDescription() => Assert.Null((await ReadProduct()).Description);

    [Then("el producto creado pertenece a la categoría {string}")]
    public async Task ThenTheProductBelongsTo(string category) => AssertCategory(await ReadProduct(), category);

    [Then("el producto se actualiza correctamente")]
    public void ThenTheProductIsUpdated() => Assert.Equal(HttpStatusCode.OK, context.Response.StatusCode);

    [Then("el producto actualizado tiene el nombre {string}")]
    public Task ThenTheUpdatedProductHasName(string name) => ThenTheProductHasName(name);

    [Then("el producto se elimina correctamente")]
    public void ThenTheProductIsDeleted() => Assert.Equal(HttpStatusCode.NoContent, context.Response.StatusCode);

    [Then("al consultar el detalle del producto {string} tiene la descripción {string}")]
    public async Task ThenTheStoredProductHasDescription(string name, string description) =>
        Assert.Equal(description, (await GetStoredProduct(name)).Description);

    [Then("al consultar el detalle del producto {string} no tiene descripción")]
    public async Task ThenTheStoredProductHasNoDescription(string name) =>
        Assert.Null((await GetStoredProduct(name)).Description);

    [Then("al consultar el detalle del producto {string} muestra el id y el nombre de la categoría {string}")]
    public async Task ThenTheStoredProductShowsCategory(string name, string category) =>
        AssertCategory(await GetStoredProduct(name), category);

    [Then("al consultar el detalle del producto {string} se encuentra")]
    public Task ThenTheStoredProductIsFound(string name) => GetStoredProduct(name);

    [Then("al consultar el detalle del producto eliminado no se encuentra")]
    public async Task ThenTheDeletedProductIsNotFound()
    {
        var response = await client.GetAsync($"{ProductsUrl}/{_deletedProductId}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // Consulta el detalle sin reemplazar la última respuesta, para comprobar lo que quedó guardado.
    private async Task<ProductResponse> GetStoredProduct(string name)
    {
        var product = await client.GetFromJsonAsync<ProductResponse>($"{ProductsUrl}/{context.ProductIds[name]}");
        Assert.NotNull(product);
        Assert.Equal(name, product.Name);
        return product;
    }

    [Then("el producto se encuentra")]
    public void ThenTheProductIsFound() => Assert.Equal(HttpStatusCode.OK, context.Response.StatusCode);

    [Then("el producto no se encuentra")]
    public void ThenTheProductIsNotFound() => Assert.Equal(HttpStatusCode.NotFound, context.Response.StatusCode);

    [Then("el producto consultado tiene el nombre {string}")]
    public Task ThenTheRetrievedProductHasName(string name) => ThenTheProductHasName(name);

    [Then("el producto consultado tiene la descripción {string}")]
    public Task ThenTheRetrievedProductHasDescription(string description) => ThenTheProductHasDescription(description);

    [Then("el producto consultado muestra el id y el nombre de la categoría {string}")]
    public async Task ThenTheRetrievedProductShowsCategory(string category) =>
        AssertCategory(await ReadProduct(), category);

    [Then("la lista contiene {int} productos de un total de {int}")]
    public async Task ThenTheListContains(int count, int totalCount)
    {
        Assert.Equal(HttpStatusCode.OK, context.Response.StatusCode);

        var page = await ReadProductPage();
        Assert.Equal(count, page.Items.Count);
        Assert.Equal(totalCount, page.TotalCount);
    }

    [Then("la lista corresponde a la página {int} con {int} productos por página")]
    public async Task ThenTheListIsPage(int pageNumber, int pageSize)
    {
        var page = await ReadProductPage();
        Assert.Equal(pageNumber, page.Page);
        Assert.Equal(pageSize, page.PageSize);
    }

    [Then("la lista muestra los productos en este orden:")]
    public async Task ThenTheListShowsInOrder(DataTable table)
    {
        var expected = table.Rows.Select(row => (row["nombre"], EmptyAsNull(row["descripción"]), row["categoría"]));

        var page = await ReadProductPage();
        Assert.Equal(expected, page.Items.Select(p => (p.Name, p.Description, p.Category.Name)));
    }

    private void AssertCategory(ProductResponse product, string category) =>
        Assert.Equal(new CategorySummary(context.CategoryIds[category], category), product.Category);

    private Task<ProductResponse> ReadProduct() => context.ReadBodyAsync<ProductResponse>();

    private Task<PagedResponse<ProductResponse>> ReadProductPage() =>
        context.ReadBodyAsync<PagedResponse<ProductResponse>>();
}
