using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdvertisementProductLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvertisementProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisementId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementProducts_Advertisements_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvertisementProducts_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementProducts_AdvertisementId_ProductId",
                table: "AdvertisementProducts",
                columns: new[] { "AdvertisementId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementProducts_ProductId",
                table: "AdvertisementProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementProducts");
        }
    }
}
