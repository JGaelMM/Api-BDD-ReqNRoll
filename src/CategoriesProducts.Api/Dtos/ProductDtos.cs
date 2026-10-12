namespace CategoriesProducts.Api.Dtos;

/// <summary>Datos para crear o editar un producto.</summary>
public record ProductRequest(string? Name, string? Description, int? CategoryId);

public record ProductResponse(int Id, string Name, string? Description, CategorySummary Category);

/// <summary>La categoría del producto: su id y su nombre.</summary>
public record CategorySummary(int Id, string Name);
