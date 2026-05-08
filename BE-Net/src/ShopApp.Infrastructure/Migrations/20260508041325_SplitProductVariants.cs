using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitProductVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Variants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ActualPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    DiscountedPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountedCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ProductLink = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DownloadUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Variants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariantOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantOptions_Variants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "Variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VariantOptions_VariantId_Name",
                table: "VariantOptions",
                columns: new[] { "VariantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Variants_ProductId_IsDefault",
                table: "Variants",
                columns: new[] { "ProductId", "IsDefault" });

            migrationBuilder.Sql("""
                INSERT INTO "Variants" (
                    "Id", "ProductId", "Name", "ActualPrice", "Currency", "DiscountedPrice", "DiscountedCurrency",
                    "DiscountPercentage", "ProductLink", "DownloadUrl", "IsDefault", "CreatedAt", "UpdatedAt")
                SELECT
                    "Id", "Id", 'Default', "ActualPrice", "Currency", "DiscountedPrice", "DiscountedCurrency",
                    "DiscountPercentage", "ProductLink", "DownloadUrl", TRUE, "CreatedAt", "UpdatedAt"
                FROM "Products";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "VariantOptions" ("Id", "VariantId", "Name", "Value", "CreatedAt", "UpdatedAt")
                SELECT "Id", "Id", 'Edition', 'Standard', "CreatedAt", "UpdatedAt"
                FROM "Products";
                """);

            migrationBuilder.DropColumn(
                name: "ActualPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountedCurrency",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductLink",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualPrice",
                table: "Products",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Products",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Products",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DiscountedCurrency",
                table: "Products",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Products",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                table: "Products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductLink",
                table: "Products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Products" p
                SET
                    "ActualPrice" = v."ActualPrice",
                    "Currency" = v."Currency",
                    "DiscountPercentage" = v."DiscountPercentage",
                    "DiscountedCurrency" = v."DiscountedCurrency",
                    "DiscountedPrice" = v."DiscountedPrice",
                    "DownloadUrl" = v."DownloadUrl",
                    "ProductLink" = v."ProductLink"
                FROM "Variants" v
                WHERE v."ProductId" = p."Id" AND v."IsDefault" = TRUE;
                """);

            migrationBuilder.DropTable(
                name: "VariantOptions");

            migrationBuilder.DropTable(
                name: "Variants");
        }
    }
}
