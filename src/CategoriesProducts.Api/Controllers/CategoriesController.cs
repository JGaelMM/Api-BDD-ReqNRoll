using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesProducts.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(CategoryService categoryService) : ControllerBase
{
    /// <summary>Lista las categorías ordenadas por nombre, de forma paginada.</summary>
    /// <param name="page">Número de página (desde 1).</param>
    /// <param name="pageSize">Categorías por página (de 1 a 100).</param>
    [HttpGet]
    [ProducesResponseType<PagedResponse<CategoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<PagedResponse<CategoryResponse>> GetPage(
        CancellationToken cancellationToken, int page = 1, int pageSize = Pagination.DefaultPageSize) =>
        categoryService.GetPageAsync(page, pageSize, cancellationToken);

    /// <summary>Muestra el detalle de una categoría.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<CategoryResponse> GetById(int id, CancellationToken cancellationToken) =>
        categoryService.GetByIdAsync(id, cancellationToken);

    /// <summary>Crea una categoría. El nombre es requerido y no se puede repetir (sin importar mayúsculas/minúsculas).</summary>
    [HttpPost]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>Edita el nombre y la descripción de una categoría. Aplican las mismas reglas que al crear.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public Task<CategoryResponse> Update(int id, CategoryRequest request, CancellationToken cancellationToken) =>
        categoryService.UpdateAsync(id, request, cancellationToken);

    /// <summary>Elimina una categoría. No se puede eliminar si tiene productos asignados.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await categoryService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
