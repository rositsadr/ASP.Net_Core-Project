using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class ChangeTasteToName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Taste",
                table: "ProductTastes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "ProductColors",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductTastes",
                newName: "Taste");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductColors",
                newName: "Color");
        }
    }
}
