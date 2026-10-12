using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesProducts.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(CategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<PagedResponse<CategoryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<PagedResponse<CategoryResponse>> GetPage(
        CancellationToken cancellationToken, int page = 1, int pageSize = Pagination.DefaultPageSize) =>
        categoryService.GetPageAsync(page, pageSize, cancellationToken);

    [HttpGet("{id:int}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<CategoryResponse> GetById(int id, CancellationToken cancellationToken) =>
        categoryService.GetByIdAsync(id, cancellationToken);

    [HttpPost]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }
}
