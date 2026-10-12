using CategoriesProducts.Api.Dtos;
using CategoriesProducts.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesProducts.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(ProductService productService) : ControllerBase
{
    /// <summary>Lista los productos ordenados por nombre, de forma paginada, con el id y el nombre de su categoría.</summary>
    /// <param name="page">Número de página (desde 1).</param>
    /// <param name="pageSize">Productos por página (de 1 a 100).</param>
    [HttpGet]
    [ProducesResponseType<PagedResponse<ProductResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<PagedResponse<ProductResponse>> GetPage(
        CancellationToken cancellationToken, int page = 1, int pageSize = Pagination.DefaultPageSize) =>
        productService.GetPageAsync(page, pageSize, cancellationToken);

    /// <summary>Muestra el detalle de un producto con el id y el nombre de su categoría.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ProductResponse> GetById(int id, CancellationToken cancellationToken) =>
        productService.GetByIdAsync(id, cancellationToken);

    /// <summary>Crea un producto. El nombre y la categoría son requeridos; la descripción es opcional.</summary>
    [HttpPost]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create(ProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Edita un producto, incluida su categoría.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<ProductResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public Task<ProductResponse> Update(int id, ProductRequest request, CancellationToken cancellationToken) =>
        productService.UpdateAsync(id, request, cancellationToken);

    /// <summary>Elimina un producto.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await productService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
