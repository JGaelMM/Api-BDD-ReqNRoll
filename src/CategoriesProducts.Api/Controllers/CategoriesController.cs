using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesProducts.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(CategoryService categoryService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateAsync(request, cancellationToken);
        return Created($"/api/categories/{category.Id}", category);
    }
}
