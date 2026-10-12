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
    [InlineData("Coca-Cola")]
    [InlineData("coca-cola")]
    [InlineData("  COCA-COLA  ")]
    public void NormalizedName_IsTheSameRegardlessOfCaseAndSurroundingSpaces(string name)
    {
        var product = new Product { Name = name };

        Assert.Equal("COCA-COLA", product.NormalizedName);
    }

    [Fact]
    public void NormalizedName_IsUpdatedWhenNameChanges()
    {
        var product = new Product { Name = "Coca-Cola" };

        product.Name = "Agua Ciel";

        Assert.Equal("AGUA CIEL", product.NormalizedName);
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
