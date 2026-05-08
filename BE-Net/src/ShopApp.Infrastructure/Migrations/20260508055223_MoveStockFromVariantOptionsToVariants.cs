using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveStockFromVariantOptionsToVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "Variants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE "Variants" v
                SET "Stock" = COALESCE((
                    SELECT MAX(o."Stock")
                    FROM "VariantOptions" o
                    WHERE o."VariantId" = v."Id"
                ), 0);
                """);

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "VariantOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Stock",
                table: "VariantOptions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE "VariantOptions" o
                SET "Stock" = v."Stock"
                FROM "Variants" v
                WHERE o."VariantId" = v."Id";
                """);

            migrationBuilder.DropColumn(
                name: "Stock",
                table: "Variants");
        }
    }
}
