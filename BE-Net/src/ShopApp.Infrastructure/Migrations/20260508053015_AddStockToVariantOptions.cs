using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockToVariantOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "VariantOptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE "VariantOptions"
                SET "Stock" = CASE
                    WHEN "Name" = 'Edition' AND "Value" = 'Standard' THEN 100
                    WHEN "Name" = 'Edition' THEN 40
                    ELSE 25
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stock",
                table: "VariantOptions");
        }
    }
}
