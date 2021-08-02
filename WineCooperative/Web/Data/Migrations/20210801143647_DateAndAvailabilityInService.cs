using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class DateAndAvailabilityInService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Available",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "DateCreated",
                table: "Services",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WineAreas_Name",
                table: "WineAreas",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrapeVarieties_Name",
                table: "GrapeVarieties",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WineAreas_Name",
                table: "WineAreas");

            migrationBuilder.DropIndex(
                name: "IX_GrapeVarieties_Name",
                table: "GrapeVarieties");

            migrationBuilder.DropColumn(
                name: "Available",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Services");
        }
    }
}
