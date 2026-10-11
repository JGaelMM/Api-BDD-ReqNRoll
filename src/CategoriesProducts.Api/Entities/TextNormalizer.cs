namespace CategoriesProducts.Api.Entities;

public static class TextNormalizer
{
    /// <summary>Quita los espacios de los extremos; un texto vacío se guarda como null.</summary>
    public static string? OptionalText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
