using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingSummaryToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FiveStarCount",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FourStarCount",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OneStarCount",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ThreeStarCount",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TwoStarCount",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                WITH review_summary AS (
                    SELECT
                        "ProductId",
                        ROUND(AVG("Rating"), 1) AS "Average",
                        COUNT(*)::integer AS "TotalCount",
                        COUNT(*) FILTER (WHERE ROUND("Rating") = 1)::integer AS "OneStarCount",
                        COUNT(*) FILTER (WHERE ROUND("Rating") = 2)::integer AS "TwoStarCount",
                        COUNT(*) FILTER (WHERE ROUND("Rating") = 3)::integer AS "ThreeStarCount",
                        COUNT(*) FILTER (WHERE ROUND("Rating") = 4)::integer AS "FourStarCount",
                        COUNT(*) FILTER (WHERE ROUND("Rating") >= 5)::integer AS "FiveStarCount"
                    FROM "Reviews"
                    GROUP BY "ProductId"
                )
                UPDATE "Products" p
                SET
                    "Rating" = s."Average",
                    "RatingCount" = s."TotalCount",
                    "OneStarCount" = s."OneStarCount",
                    "TwoStarCount" = s."TwoStarCount",
                    "ThreeStarCount" = s."ThreeStarCount",
                    "FourStarCount" = s."FourStarCount",
                    "FiveStarCount" = s."FiveStarCount"
                FROM review_summary s
                WHERE p."Id" = s."ProductId";
                """);

            migrationBuilder.Sql("""
                UPDATE "Products"
                SET
                    "OneStarCount" = CASE WHEN ROUND("Rating") = 1 THEN "RatingCount" ELSE 0 END,
                    "TwoStarCount" = CASE WHEN ROUND("Rating") = 2 THEN "RatingCount" ELSE 0 END,
                    "ThreeStarCount" = CASE WHEN ROUND("Rating") = 3 THEN "RatingCount" ELSE 0 END,
                    "FourStarCount" = CASE WHEN ROUND("Rating") = 4 THEN "RatingCount" ELSE 0 END,
                    "FiveStarCount" = CASE WHEN ROUND("Rating") >= 5 THEN "RatingCount" ELSE 0 END
                WHERE "RatingCount" > 0
                  AND ("OneStarCount" + "TwoStarCount" + "ThreeStarCount" + "FourStarCount" + "FiveStarCount") = 0;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FiveStarCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FourStarCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OneStarCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ThreeStarCount",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TwoStarCount",
                table: "Products");
        }
    }
}
