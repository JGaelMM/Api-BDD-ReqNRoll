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
        set => _name = value.Trim();
    }

    public string? Description
    {
        get => _description;
        set => _description = TextNormalizer.OptionalText(value);
    }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}
