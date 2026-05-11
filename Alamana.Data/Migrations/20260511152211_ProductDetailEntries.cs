using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductDetailEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductDetailEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    EntryKey = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    EntryValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetailEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetailEntries_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetailEntries_ProductId",
                table: "ProductDetailEntries",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDetailEntries");
        }
    }
}
