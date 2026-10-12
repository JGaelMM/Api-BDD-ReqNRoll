using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoriesProducts.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductNormalizedName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE Products SET NormalizedName = UPPER(LTRIM(RTRIM(Name)))");

            migrationBuilder.CreateIndex(
                name: "IX_Products_NormalizedName",
                table: "Products",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_NormalizedName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Products");
        }
    }
}
