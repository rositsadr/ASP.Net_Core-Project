using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Data.Migrations
{
    public partial class RemoveOrdersFromUserInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserAdditionalInformation_UserAdditionalInformationId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserAdditionalInformationId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserAdditionalInformationId",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserAdditionalInformationId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserAdditionalInformationId",
                table: "Orders",
                column: "UserAdditionalInformationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserAdditionalInformation_UserAdditionalInformationId",
                table: "Orders",
                column: "UserAdditionalInformationId",
                principalTable: "UserAdditionalInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
