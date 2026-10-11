using CategoriesProducts.Api.Errors;
using CategoriesProducts.Api.Services;

namespace CategoriesProducts.UnitTests.Services;

public class FieldValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateName_WithoutName_Throws(string? name)
    {
        var error = Assert.Throws<ValidationException>(() => FieldValidator.ValidateName(name, 100));

        Assert.Equal("El nombre es requerido.", error.Message);
    }

    [Fact]
    public void ValidateName_LongerThanMax_Throws()
    {
        var error = Assert.Throws<ValidationException>(() => FieldValidator.ValidateName(new string('a', 101), 100));

        Assert.Equal("El nombre no puede tener más de 100 caracteres.", error.Message);
    }

    [Fact]
    public void ValidateName_IgnoresSurroundingSpacesForMaxLength()
    {
        FieldValidator.ValidateName($"  {new string('a', 100)}  ", 100);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Refrescos y jugos")]
    public void ValidateDescription_OptionalAndWithinMax_DoesNotThrow(string? description)
    {
        FieldValidator.ValidateDescription(description, 500);
    }

    [Fact]
    public void ValidateDescription_LongerThanMax_Throws()
    {
        var error = Assert.Throws<ValidationException>(() => FieldValidator.ValidateDescription(new string('a', 501), 500));

        Assert.Equal("La descripción no puede tener más de 500 caracteres.", error.Message);
    }
}
