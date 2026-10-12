using System.Linq.Expressions;
using CategoriesProducts.Api.Data;
using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Entities;
using CategoriesProducts.Api.Errors;
using Microsoft.EntityFrameworkCore;

namespace CategoriesProducts.Api.Services;

public class ProductService(AppDbContext db)
{
    // Proyección a la respuesta: EF la traduce a un JOIN con Categories para obtener el nombre de la categoría.
    private static readonly Expression<Func<Product, ProductResponse>> ToResponse = p =>
        new ProductResponse(p.Id, p.Name, p.Description, new CategorySummary(p.CategoryId, p.Category.Name));

    public Task<PagedResponse<ProductResponse>> GetPageAsync(
        int page, int pageSize, CancellationToken cancellationToken = default) =>
        db.Products
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .Select(ToResponse)
            .ToPagedResponseAsync(page, pageSize, cancellationToken);

    public async Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        await db.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(ToResponse)
            .FirstOrDefaultAsync(cancellationToken)
        ?? throw ProductNotFound();

    public async Task<ProductResponse> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        var category = await ValidateAsync(request, cancellationToken);

        var product = new Product { Name = request.Name!, Description = request.Description, Category = category };
        await EnsureNameIsAvailableAsync(product, cancellationToken);

        db.Products.Add(product);
        await SaveAsync(product, cancellationToken);

        return ToResponseFrom(product);
    }

    public async Task<ProductResponse> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var category = await ValidateAsync(request, cancellationToken);

        var product = await FindAsync(id, cancellationToken);
        product.Name = request.Name!;
        product.Description = request.Description;
        product.Category = category;
        await EnsureNameIsAvailableAsync(product, cancellationToken);

        await SaveAsync(product, cancellationToken);

        return ToResponseFrom(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await FindAsync(id, cancellationToken);

        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<Product> FindAsync(int id, CancellationToken cancellationToken) =>
        await db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken) ?? throw ProductNotFound();

    private static ProductResponse ToResponseFrom(Product product) =>
        new(product.Id, product.Name, product.Description, new CategorySummary(product.Category.Id, product.Category.Name));

    /// <summary>Valida los datos y regresa la categoría indicada.</summary>
    private async Task<Category> ValidateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        FieldValidator.ValidateName(request.Name, Product.NameMaxLength);
        FieldValidator.ValidateDescription(request.Description, Product.DescriptionMaxLength);

        if (request.CategoryId is not { } categoryId)
        {
            throw new ValidationException("La categoría es requerida.");
        }

        return await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            ?? throw new ValidationException("La categoría indicada no existe.");
    }

    private async Task EnsureNameIsAvailableAsync(Product product, CancellationToken cancellationToken)
    {
        if (await IsNameTakenByAnotherProductAsync(product, cancellationToken))
        {
            throw DuplicateName(product.Name);
        }
    }

    private async Task SaveAsync(Product product, CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            if (await IsNameTakenByAnotherProductAsync(product, cancellationToken))
            {
                throw DuplicateName(product.Name);
            }

            throw;
        }
    }

    private Task<bool> IsNameTakenByAnotherProductAsync(Product product, CancellationToken cancellationToken) =>
        db.Products.AsNoTracking().AnyAsync(
            p => p.NormalizedName == product.NormalizedName && p.Id != product.Id,
            cancellationToken);

    private static ConflictException DuplicateName(string name) =>
        new($"Ya existe un producto con el nombre '{name}'.");

    private static NotFoundException ProductNotFound() => new("No se encontró el producto.");
}
