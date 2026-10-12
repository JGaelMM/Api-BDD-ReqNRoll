namespace CategoriesProducts.Api.Entities;

public class Product
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    private string _name = string.Empty;
    private string? _description;

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

    public string NormalizedName { get; private set; } = string.Empty;

    public string? Description
    {
        get => _description;
        set => _description = TextNormalizer.OptionalText(value);
    }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public static string Normalize(string name) => name.Trim().ToUpperInvariant();
}
