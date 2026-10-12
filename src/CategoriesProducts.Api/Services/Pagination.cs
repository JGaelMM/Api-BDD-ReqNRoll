using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Errors;
using Microsoft.EntityFrameworkCore;

namespace CategoriesProducts.Api.Services;

public static class Pagination
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public static void Validate(int page, int pageSize)
    {
        if (page < 1)
        {
            throw new ValidationException("La página debe ser mayor o igual a 1.");
        }

        if (pageSize is < 1 or > MaxPageSize)
        {
            throw new ValidationException($"El tamaño de página debe estar entre 1 y {MaxPageSize}.");
        }
    }

    /// <summary>Valida los parámetros y obtiene la página pedida de una consulta ya ordenada.</summary>
    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> orderedQuery, int page, int pageSize, CancellationToken cancellationToken)
    {
        Validate(page, pageSize);

        var totalCount = await orderedQuery.CountAsync(cancellationToken);
        var items = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<T>(items, page, pageSize, totalCount);
    }
}
