using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class BilingualProductDetailEntries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntryValue",
                table: "ProductDetailEntries",
                newName: "EntryValueEn");

            migrationBuilder.RenameColumn(
                name: "EntryKey",
                table: "ProductDetailEntries",
                newName: "EntryKeyEn");

            migrationBuilder.AddColumn<string>(
                name: "EntryKeyAr",
                table: "ProductDetailEntries",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntryValueAr",
                table: "ProductDetailEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntryKeyAr",
                table: "ProductDetailEntries");

            migrationBuilder.DropColumn(
                name: "EntryValueAr",
                table: "ProductDetailEntries");

            migrationBuilder.RenameColumn(
                name: "EntryValueEn",
                table: "ProductDetailEntries",
                newName: "EntryValue");

            migrationBuilder.RenameColumn(
                name: "EntryKeyEn",
                table: "ProductDetailEntries",
                newName: "EntryKey");
        }
    }
}
