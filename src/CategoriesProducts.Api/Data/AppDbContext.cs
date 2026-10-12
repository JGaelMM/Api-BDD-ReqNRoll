using CategoriesProducts.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoriesProducts.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(category =>
        {
            category.Property(c => c.Name).HasMaxLength(Category.NameMaxLength).IsRequired();
            category.Property(c => c.NormalizedName).HasMaxLength(Category.NameMaxLength).IsRequired();
            category.Property(c => c.Description).HasMaxLength(Category.DescriptionMaxLength);
            category.HasIndex(c => c.NormalizedName).IsUnique();
        });

        modelBuilder.Entity<Product>(product =>
        {
            product.Property(p => p.Name).HasMaxLength(Product.NameMaxLength).IsRequired();
            product.Property(p => p.NormalizedName).HasMaxLength(Product.NameMaxLength).IsRequired();
            product.HasIndex(p => p.NormalizedName).IsUnique();
            product.Property(p => p.Description).HasMaxLength(Product.DescriptionMaxLength);

            // Restrict: la base de datos tampoco permite borrar una categoría que tenga productos.
            product.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
