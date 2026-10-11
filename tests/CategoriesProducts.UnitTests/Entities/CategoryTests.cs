using CategoriesProducts.Api.Entities;

namespace CategoriesProducts.UnitTests.Entities;

public class CategoryTests
{
    [Fact]
    public void Name_IsTrimmed()
    {
        var category = new Category { Name = "  Bebidas  " };

        Assert.Equal("Bebidas", category.Name);
    }

    [Theory]
    [InlineData("Bebidas")]
    [InlineData("bebidas")]
    [InlineData("BEBIDAS")]
    [InlineData("  bEbIdAs  ")]
    public void NormalizedName_IsTheSameRegardlessOfCaseAndSurroundingSpaces(string name)
    {
        var category = new Category { Name = name };

        Assert.Equal("BEBIDAS", category.NormalizedName);
    }

    [Fact]
    public void NormalizedName_SupportsAccentedCharacters()
    {
        var category = new Category { Name = "lácteos" };

        Assert.Equal("LÁCTEOS", category.NormalizedName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Description_WhenEmpty_IsNull(string? description)
    {
        var category = new Category { Name = "Bebidas", Description = description };

        Assert.Null(category.Description);
    }

    [Fact]
    public void Description_IsTrimmed()
    {
        var category = new Category { Name = "Bebidas", Description = "  Refrescos  " };

        Assert.Equal("Refrescos", category.Description);
    }

    [Fact]
    public void NormalizedName_IsUpdatedWhenNameChanges()
    {
        var category = new Category { Name = "Bebidas" };

        category.Name = "Lácteos";

        Assert.Equal("LÁCTEOS", category.NormalizedName);
    }
}
