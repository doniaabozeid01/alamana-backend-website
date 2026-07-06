using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryBilingualCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "currency",
                table: "Country",
                newName: "currency_en");

            migrationBuilder.AddColumn<string>(
                name: "currency_ar",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currency_ar",
                table: "Country");

            migrationBuilder.RenameColumn(
                name: "currency_en",
                table: "Country",
                newName: "currency");
        }
    }
}
