using CategoriesProducts.Api.Entities;

namespace CategoriesProducts.Api.Dtos;

/// <summary>Datos para crear o editar una categoría.</summary>
public record CategoryRequest(string? Name, string? Description);

public record CategoryResponse(int Id, string Name, string? Description)
{
    public static CategoryResponse From(Category category) => new(category.Id, category.Name, category.Description);
}
