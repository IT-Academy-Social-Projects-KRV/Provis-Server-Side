using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class AddSeedUserRoleTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoleTags",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Worker" });

            migrationBuilder.InsertData(
                table: "UserRoleTags",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Support" });

            migrationBuilder.InsertData(
                table: "UserRoleTags",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3, "Reviewer" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoleTags",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRoleTags",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoleTags",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
