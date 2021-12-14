using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class AddSeedForWorkspaceRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorkspaceRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Owner" },
                    { 2, "Manager" },
                    { 3, "Member" },
                    { 4, "Viewer" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorkspaceRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WorkspaceRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WorkspaceRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "WorkspaceRoles",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
