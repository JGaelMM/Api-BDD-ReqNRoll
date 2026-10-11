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
}
