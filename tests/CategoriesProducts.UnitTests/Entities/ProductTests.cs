using CategoriesProducts.Api.Entities;

namespace CategoriesProducts.UnitTests.Entities;

public class ProductTests
{
    [Fact]
    public void Name_IsTrimmed()
    {
        var product = new Product { Name = "  Coca-Cola  " };

        Assert.Equal("Coca-Cola", product.Name);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("   ", null)]
    [InlineData("  Refresco de cola  ", "Refresco de cola")]
    public void Description_IsTrimmedAndNullWhenEmpty(string? description, string? expected)
    {
        var product = new Product { Name = "Coca-Cola", Description = description };

        Assert.Equal(expected, product.Description);
    }
}
