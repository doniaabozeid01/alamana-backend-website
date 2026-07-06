using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class BilingualCatalogAndRemoveProductDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Product",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Product",
                newName: "NameAr");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Category",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Category",
                newName: "NameAr");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Advertisements",
                newName: "TitleEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Advertisements",
                newName: "TitleAr");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Category",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionEn",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "DescriptionEn",
                table: "Advertisements");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Product",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NameAr",
                table: "Product",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Category",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "NameAr",
                table: "Category",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "TitleEn",
                table: "Advertisements",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "TitleAr",
                table: "Advertisements",
                newName: "Description");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
