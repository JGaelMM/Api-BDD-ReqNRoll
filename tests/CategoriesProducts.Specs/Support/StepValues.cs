namespace CategoriesProducts.Specs.Support;

public static class StepValues
{
    /// <summary>Id que nunca existe en la base de datos de pruebas.</summary>
    public const int NonExistentId = int.MaxValue;

    /// <summary>Una celda vacía en una tabla de Gherkin significa "sin valor".</summary>
    public static string? EmptyAsNull(string value) => value.Length == 0 ? null : value;
}
