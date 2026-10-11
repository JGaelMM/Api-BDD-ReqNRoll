namespace CategoriesProducts.Api.Entities;

public class Category
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    private string _name = string.Empty;

    public int Id { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            _name = value.Trim();
            NormalizedName = Normalize(_name);
        }
    }

    // Copia del nombre en mayúsculas con índice único: así la unicidad no depende de la collation de la base de datos.
    public string NormalizedName { get; private set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Product> Products { get; } = [];

    public static string Normalize(string name) => name.Trim().ToUpperInvariant();
}
