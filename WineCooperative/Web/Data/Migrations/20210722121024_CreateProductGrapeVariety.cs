using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class CreateProductGrapeVariety : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrapeVarietyProduct");

            migrationBuilder.CreateTable(
                name: "ProductGrapeVarieties",
                columns: table => new
                {
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GrapeVarietyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductGrapeVarieties", x => new { x.ProductId, x.GrapeVarietyId });
                    table.ForeignKey(
                        name: "FK_ProductGrapeVarieties_GrapeVarieties_GrapeVarietyId",
                        column: x => x.GrapeVarietyId,
                        principalTable: "GrapeVarieties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductGrapeVarieties_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductGrapeVarieties_GrapeVarietyId",
                table: "ProductGrapeVarieties",
                column: "GrapeVarietyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductGrapeVarieties");

            migrationBuilder.CreateTable(
                name: "GrapeVarietyProduct",
                columns: table => new
                {
                    GrapeVarietiesId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrapeVarietyProduct", x => new { x.GrapeVarietiesId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_GrapeVarietyProduct_GrapeVarieties_GrapeVarietiesId",
                        column: x => x.GrapeVarietiesId,
                        principalTable: "GrapeVarieties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrapeVarietyProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrapeVarietyProduct_ProductsId",
                table: "GrapeVarietyProduct",
                column: "ProductsId");
        }
    }
}
