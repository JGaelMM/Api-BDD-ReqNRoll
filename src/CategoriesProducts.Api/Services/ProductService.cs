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
        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);

        return ToResponseFrom(product);
    }

    public async Task<ProductResponse> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var category = await ValidateAsync(request, cancellationToken);

        var product = await FindAsync(id, cancellationToken);
        product.Name = request.Name!;
        product.Description = request.Description;
        product.Category = category;
        await db.SaveChangesAsync(cancellationToken);

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

    private static NotFoundException ProductNotFound() => new("No se encontró el producto.");
}
