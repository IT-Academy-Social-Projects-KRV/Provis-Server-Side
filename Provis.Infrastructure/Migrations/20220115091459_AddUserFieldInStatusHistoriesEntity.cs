using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class AddUserFieldInStatusHistoriesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StatusHistories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusHistories_UserId",
                table: "StatusHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatusHistories_AspNetUsers_UserId",
                table: "StatusHistories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatusHistories_AspNetUsers_UserId",
                table: "StatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_StatusHistories_UserId",
                table: "StatusHistories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StatusHistories");
        }
    }
}
