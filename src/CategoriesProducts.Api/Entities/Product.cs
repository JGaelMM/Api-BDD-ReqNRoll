namespace CategoriesProducts.Api.Entities;

public class Product
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;

    private string _name = string.Empty;

    public int Id { get; set; }

    public string Name
    {
        get => _name;
        set => _name = value.Trim();
    }

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}
