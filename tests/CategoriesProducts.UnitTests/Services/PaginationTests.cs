using CategoriesProducts.Api.Errors;
using CategoriesProducts.Api.Services;

namespace CategoriesProducts.UnitTests.Services;

public class PaginationTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 20)]
    [InlineData(5, 100)]
    public void Validate_WithValidValues_DoesNotThrow(int page, int pageSize)
    {
        Pagination.Validate(page, pageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_PageLessThanOne_Throws(int page)
    {
        var error = Assert.Throws<ValidationException>(() => Pagination.Validate(page, 20));

        Assert.Equal("La página debe ser mayor o igual a 1.", error.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_PageSizeOutOfRange_Throws(int pageSize)
    {
        var error = Assert.Throws<ValidationException>(() => Pagination.Validate(1, pageSize));

        Assert.Equal("El tamaño de página debe estar entre 1 y 100.", error.Message);
    }
}
