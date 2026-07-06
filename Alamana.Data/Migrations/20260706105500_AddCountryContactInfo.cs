using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alamana.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryContactInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cart_userId",
                table: "Cart");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "office_address",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone2",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "working_hours",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "office_address",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "phone2",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "working_hours",
                table: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_userId",
                table: "Cart",
                column: "userId");
        }
    }
}
