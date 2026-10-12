using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Errors;
using CategoriesProducts.Api.Services;
using CategoriesProducts.UnitTests.Support;

namespace CategoriesProducts.UnitTests.Services;

public sealed class CategoryServiceTests : IDisposable
{
    private readonly SqliteDatabase _database = new();

    public void Dispose() => _database.Dispose();

    private CategoryService CreateService() => new(_database.CreateContext());

    [Fact]
    public async Task CreateAsync_SavesAndReturnsTheCategory()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("  Bebidas  ", "Refrescos"));

        Assert.True(created.Id > 0);
        Assert.Equal("Bebidas", created.Name);
        Assert.Equal("Refrescos", created.Description);

        using var db = _database.CreateContext();
        var saved = await db.Categories.FindAsync(created.Id);
        Assert.NotNull(saved);
        Assert.Equal("BEBIDAS", saved.NormalizedName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateAsync_WithoutDescription_SavesNullDescription(string? description)
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", description));

        Assert.Null(created.Description);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidName_ThrowsValidationAndDoesNotSave()
    {
        await Assert.ThrowsAsync<ValidationException>(() => CreateService().CreateAsync(new CategoryRequest(" ", null)));

        using var db = _database.CreateContext();
        Assert.Empty(db.Categories);
    }

    [Theory]
    [InlineData("Bebidas")]
    [InlineData("BEBIDAS")]
    [InlineData("  bebidas  ")]
    public async Task CreateAsync_WithExistingName_ThrowsConflict(string name)
    {
        await CreateService().CreateAsync(new CategoryRequest("Bebidas", null));

        var error = await Assert.ThrowsAsync<ConflictException>(
            () => CreateService().CreateAsync(new CategoryRequest(name, null)));

        Assert.Equal($"Ya existe una categoría con el nombre '{name.Trim()}'.", error.Message);
    }

    [Fact]
    public async Task UpdateAsync_ChangesNameAndDescription()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", "Refrescos"));

        var updated = await CreateService().UpdateAsync(created.Id, new CategoryRequest("  Bebidas frías  ", null));

        Assert.Equal(new CategoryResponse(created.Id, "Bebidas frías", null), updated);
        Assert.Equal(updated, await CreateService().GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task UpdateAsync_CanKeepItsOwnNameWithDifferentCase()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", null));

        var updated = await CreateService().UpdateAsync(created.Id, new CategoryRequest("BEBIDAS", null));

        Assert.Equal("BEBIDAS", updated.Name);
    }

    [Fact]
    public async Task UpdateAsync_WithTheNameOfAnotherCategory_ThrowsConflictAndDoesNotChange()
    {
        var service = CreateService();
        var bebidas = await service.CreateAsync(new CategoryRequest("Bebidas", null));
        await service.CreateAsync(new CategoryRequest("Lácteos", null));

        var error = await Assert.ThrowsAsync<ConflictException>(
            () => CreateService().UpdateAsync(bebidas.Id, new CategoryRequest("lácteos", null)));

        Assert.Equal("Ya existe una categoría con el nombre 'lácteos'.", error.Message);
        Assert.Equal("Bebidas", (await CreateService().GetByIdAsync(bebidas.Id)).Name);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidName_ThrowsValidation()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", null));

        await Assert.ThrowsAsync<ValidationException>(
            () => CreateService().UpdateAsync(created.Id, new CategoryRequest("", null)));
    }

    [Fact]
    public async Task UpdateAsync_WhenItDoesNotExist_ThrowsNotFound()
    {
        var error = await Assert.ThrowsAsync<NotFoundException>(
            () => CreateService().UpdateAsync(999, new CategoryRequest("Bebidas", null)));

        Assert.Equal("No se encontró la categoría.", error.Message);
    }

    [Fact]
    public async Task DeleteAsync_WithoutProducts_RemovesTheCategory()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", null));

        await CreateService().DeleteAsync(created.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService().GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteAsync_WithProducts_ThrowsConflictAndKeepsTheCategory()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", null));
        await new ProductService(_database.CreateContext()).CreateAsync(new ProductRequest("Coca-Cola", null, created.Id));

        var error = await Assert.ThrowsAsync<ConflictException>(() => CreateService().DeleteAsync(created.Id));

        Assert.Equal("Esta categoría no se puede eliminar porque tiene productos asignados.", error.Message);
        Assert.Equal(created, await CreateService().GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteAsync_AfterDeletingAllItsProducts_RemovesTheCategory()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", null));
        var products = new ProductService(_database.CreateContext());
        var product = await products.CreateAsync(new ProductRequest("Coca-Cola", null, created.Id));
        await products.DeleteAsync(product.Id);

        await CreateService().DeleteAsync(created.Id);

        await Assert.ThrowsAsync<NotFoundException>(() => CreateService().GetByIdAsync(created.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenItDoesNotExist_ThrowsNotFound()
    {
        var error = await Assert.ThrowsAsync<NotFoundException>(() => CreateService().DeleteAsync(999));

        Assert.Equal("No se encontró la categoría.", error.Message);
    }

    [Fact]
    public async Task GetPageAsync_ReturnsTheRequestedPageOrderedByName()
    {
        var service = CreateService();
        foreach (var name in new[] { "Dulces", "Bebidas", "Carnes", "Abarrotes" })
        {
            await service.CreateAsync(new CategoryRequest(name, null));
        }

        var page = await CreateService().GetPageAsync(page: 2, pageSize: 2);

        Assert.Equal(["Carnes", "Dulces"], page.Items.Select(c => c.Name));
        Assert.Equal(2, page.Page);
        Assert.Equal(2, page.PageSize);
        Assert.Equal(4, page.TotalCount);
    }

    [Fact]
    public async Task GetPageAsync_WithInvalidPage_Throws()
    {
        await Assert.ThrowsAsync<ValidationException>(() => CreateService().GetPageAsync(page: 0, pageSize: 20));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTheCategory()
    {
        var created = await CreateService().CreateAsync(new CategoryRequest("Bebidas", "Refrescos"));

        var category = await CreateService().GetByIdAsync(created.Id);

        Assert.Equal(created, category);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItDoesNotExist_ThrowsNotFound()
    {
        var error = await Assert.ThrowsAsync<NotFoundException>(() => CreateService().GetByIdAsync(999));

        Assert.Equal("No se encontró la categoría.", error.Message);
    }
}
