using CategoriesProducts.Api.Errors;

namespace CategoriesProducts.Api.Services;

/// <summary>
/// Reglas de nombre y descripción, compartidas por categorías y productos.
/// </summary>
public static class FieldValidator
{
    public static void ValidateName(string? name, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("El nombre es requerido.");
        }

        if (name.Trim().Length > maxLength)
        {
            throw new ValidationException($"El nombre no puede tener más de {maxLength} caracteres.");
        }
    }

    public static void ValidateDescription(string? description, int maxLength)
    {
        if (description is not null && description.Trim().Length > maxLength)
        {
            throw new ValidationException($"La descripción no puede tener más de {maxLength} caracteres.");
        }
    }
}
