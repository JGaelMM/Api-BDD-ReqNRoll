using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Errors;
using CategoriesProducts.Api.Services;
using CategoriesProducts.UnitTests.Support;

namespace CategoriesProducts.UnitTests.Services;

public sealed class ProductServiceTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    private ProductService CreateService() => new(_database.CreateContext());

    private Task<CategoryResponse> CreateCategory(string name) =>
        new CategoryService(_database.CreateContext()).CreateAsync(new CategoryRequest(name, null));

    [Fact]
    public async Task CreateAsync_SavesAndReturnsTheProductWithItsCategory()
    {
        var bebidas = await CreateCategory("Bebidas");

        var created = await CreateService().CreateAsync(new ProductRequest("  Coca-Cola  ", "Refresco de cola", bebidas.Id));

        Assert.True(created.Id > 0);
        Assert.Equal("Coca-Cola", created.Name);
        Assert.Equal("Refresco de cola", created.Description);
        Assert.Equal(new CategorySummary(bebidas.Id, "Bebidas"), created.Category);
    }

    [Theory]
    [InlineData("Coca-Cola")]
    [InlineData("COCA-COLA")]
    [InlineData("  coca-cola  ")]
    public async Task CreateAsync_WithExistingName_ThrowsConflictAndDoesNotSave(string name)
    {
        var bebidas = await CreateCategory("Bebidas");
        await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));

        var error = await Assert.ThrowsAsync<ConflictException>(
            () => CreateService().CreateAsync(new ProductRequest(name, null, bebidas.Id)));

        Assert.Equal($"Ya existe un producto con el nombre '{name.Trim()}'.", error.Message);
        using var db = _database.CreateContext();
        Assert.Single(db.Products);
    }

    [Fact]
    public async Task CreateAsync_WithExistingNameInAnotherCategory_ThrowsConflict()
    {
        var bebidas = await CreateCategory("Bebidas");
        var refrescos = await CreateCategory("Refrescos");
        await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));

        await Assert.ThrowsAsync<ConflictException>(
            () => CreateService().CreateAsync(new ProductRequest("coca-cola", null, refrescos.Id)));
    }

    [Fact]
    public async Task UpdateAsync_CanKeepItsOwnNameWithDifferentCase()
    {
        var bebidas = await CreateCategory("Bebidas");
        var created = await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));

        var updated = await CreateService().UpdateAsync(created.Id, new ProductRequest("COCA-COLA", null, bebidas.Id));

        Assert.Equal("COCA-COLA", updated.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithTheNameOfAnotherProduct_ThrowsConflictAndDoesNotChange()
    {
        var bebidas = await CreateCategory("Bebidas");
        var cocaCola = await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));
        await CreateService().CreateAsync(new ProductRequest("Sprite", null, bebidas.Id));

        var error = await Assert.ThrowsAsync<ConflictException>(
            () => CreateService().UpdateAsync(cocaCola.Id, new ProductRequest("sprite", null, bebidas.Id)));

        Assert.Equal("Ya existe un producto con el nombre 'sprite'.", error.Message);
        Assert.Equal("Coca-Cola", (await CreateService().GetByIdAsync(cocaCola.Id)).Name);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidName_ThrowsValidation()
    {
        var bebidas = await CreateCategory("Bebidas");

        var error = await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().CreateAsync(new ProductRequest("  ", null, bebidas.Id)));

        Assert.Equal("El nombre es requerido.", error.Message);
    }

    [Fact]
    public async Task CreateAsync_WithoutCategory_ThrowsValidation()
    {
        var error = await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, null)));

        Assert.Equal("La categoría es requerida.", error.Message);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistentCategory_ThrowsValidationAndDoesNotSave()
    {
        var error = await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, 999)));

        Assert.Equal("La categoría indicada no existe.", error.Message);
        using var db = _database.CreateContext();
        Assert.Empty(db.Products);
    }

    [Fact]
    public async Task UpdateAsync_ChangesNameDescriptionAndCategory()
    {
        var bebidas = await CreateCategory("Bebidas");
        var botanas = await CreateCategory("Botanas");
        var created = await CreateService().CreateAsync(new ProductRequest("Coca-Cola", "Refresco", bebidas.Id));

        var updated = await CreateService().UpdateAsync(created.Id, new ProductRequest("  Doritos  ", null, botanas.Id));

        var expected = new ProductResponse(created.Id, "Doritos", null, new CategorySummary(botanas.Id, "Botanas"));
        Assert.Equal(expected, updated);
        Assert.Equal(expected, await CreateService().GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentCategory_ThrowsValidationAndDoesNotChange()
    {
        var bebidas = await CreateCategory("Bebidas");
        var created = await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));

        var error = await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().UpdateAsync(created.Id, new ProductRequest("Coca-Cola", null, 999)));

        Assert.Equal("La categoría indicada no existe.", error.Message);
        Assert.Equal(bebidas.Id, (await CreateService().GetByIdAsync(created.Id)).Category.Id);
    }

    [Fact]
    public async Task UpdateAsync_WithoutCategory_ThrowsValidation()
    {
        var bebidas = await CreateCategory("Bebidas");
        var created = await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));

        var error = await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().UpdateAsync(created.Id, new ProductRequest("Coca-Cola", null, null)));

        Assert.Equal("La categoría es requerida.", error.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenItDoesNotExist_ThrowsNotFound()
    {
        var bebidas = await CreateCategory("Bebidas");

        var error = await Assert.ThrowsAsync<NotFoundException>(
            () => CreateService().UpdateAsync(999, new ProductRequest("Coca-Cola", null, bebidas.Id)));

        Assert.Equal("No se encontró el producto.", error.Message);
    }

    [Fact]
    public async Task DeleteAsync_RemovesOnlyThatProduct()
    {
        var bebidas = await CreateCategory("Bebidas");
        var cocaCola = await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));
        var sprite = await CreateService().CreateAsync(new ProductRequest("Sprite", null, bebidas.Id));

        await CreateService().DeleteAsync(cocaCola.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService().GetByIdAsync(cocaCola.Id));
        Assert.Equal(sprite, await CreateService().GetByIdAsync(sprite.Id));
        Assert.Equal(bebidas, await new CategoryService(_database.CreateContext()).GetByIdAsync(bebidas.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenItDoesNotExist_ThrowsNotFound()
    {
        var error = await Assert.ThrowsAsync<NotFoundException>(() => CreateService().DeleteAsync(999));

        Assert.Equal("No se encontró el producto.", error.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTheProductWithItsCategoryName()
    {
        var bebidas = await CreateCategory("Bebidas");
        var created = await CreateService().CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));

        var product = await CreateService().GetByIdAsync(created.Id);

        Assert.Equal(created, product);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItDoesNotExist_ThrowsNotFound()
    {
        var error = await Assert.ThrowsAsync<NotFoundException>(() => CreateService().GetByIdAsync(999));

        Assert.Equal("No se encontró el producto.", error.Message);
    }

    [Fact]
    public async Task GetPageAsync_ReturnsTheRequestedPageOrderedByNameWithCategoryNames()
    {
        var bebidas = await CreateCategory("Bebidas");
        var botanas = await CreateCategory("Botanas");
        var service = CreateService();
        await service.CreateAsync(new ProductRequest("Sprite", null, bebidas.Id));
        await service.CreateAsync(new ProductRequest("Doritos", null, botanas.Id));
        await service.CreateAsync(new ProductRequest("Coca-Cola", null, bebidas.Id));

        var page = await CreateService().GetPageAsync(page: 1, pageSize: 2);

        Assert.Equal([("Coca-Cola", "Bebidas"), ("Doritos", "Botanas")], page.Items.Select(p => (p.Name, p.Category.Name)));
        Assert.Equal(3, page.TotalCount);
    }
}
