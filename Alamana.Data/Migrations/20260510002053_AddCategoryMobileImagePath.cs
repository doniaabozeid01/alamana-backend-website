using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryMobileImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MobileImagePath",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileImagePath",
                table: "Category");
        }
    }
}
