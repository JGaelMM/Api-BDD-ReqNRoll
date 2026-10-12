namespace CategoriesProducts.Api.Dtos;

/// <summary>Una página de resultados y el total de registros, para que el cliente sepa cuántas páginas hay.</summary>
public record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
