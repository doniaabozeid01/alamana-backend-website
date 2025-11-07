using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class addindexOnProductAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavouriteProducts_ProductId",
                table: "FavouriteProducts");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteProducts_ProductId_UserId",
                table: "FavouriteProducts",
                columns: new[] { "ProductId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FavouriteProducts_ProductId_UserId",
                table: "FavouriteProducts");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteProducts_ProductId",
                table: "FavouriteProducts",
                column: "ProductId");
        }
    }
}
