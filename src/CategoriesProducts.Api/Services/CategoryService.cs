using CategoriesProducts.Api.Data;
using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Entities;
using CategoriesProducts.Api.Errors;
using Microsoft.EntityFrameworkCore;

namespace CategoriesProducts.Api.Services;

public class CategoryService(AppDbContext db)
{
    public Task<PagedResponse<CategoryResponse>> GetPageAsync(
        int page, int pageSize, CancellationToken cancellationToken = default) =>
        db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Id)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.Description))
            .ToPagedResponseAsync(page, pageSize, cancellationToken);

    public async Task<CategoryResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
            ?? throw CategoryNotFound();

        return CategoryResponse.From(category);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        FieldValidator.ValidateName(request.Name, Category.NameMaxLength);
        FieldValidator.ValidateDescription(request.Description, Category.DescriptionMaxLength);

        var category = new Category { Name = request.Name!, Description = request.Description };
        await EnsureNameIsAvailableAsync(category.Name, cancellationToken);

        db.Categories.Add(category);
        await SaveAsync(category.Name, cancellationToken);

        return CategoryResponse.From(category);
    }

    private async Task EnsureNameIsAvailableAsync(string name, CancellationToken cancellationToken)
    {
        var normalizedName = Category.Normalize(name);

        if (await db.Categories.AnyAsync(c => c.NormalizedName == normalizedName, cancellationToken))
        {
            throw DuplicateName(name);
        }
    }

    // Si dos peticiones con el mismo nombre llegan al mismo tiempo, ambas pasan la validación
    // y el índice único de la base de datos rechaza la segunda: se responde igual que un duplicado.
    private async Task SaveAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            if (await NameExistsAsync(name, cancellationToken))
            {
                throw DuplicateName(name);
            }

            throw;
        }
    }

    private Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken)
    {
        var normalizedName = Category.Normalize(name);
        return db.Categories.AsNoTracking().AnyAsync(c => c.NormalizedName == normalizedName, cancellationToken);
    }

    private static ConflictException DuplicateName(string name) =>
        new($"Ya existe una categoría con el nombre '{name}'.");

    private static NotFoundException CategoryNotFound() => new("No se encontró la categoría.");
}
