using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class AddUpdateUserTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskRoles", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "TaskRoleId",
                table: "UsersTasks",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTasks_UserRoleTags_UserRoleTagId",
                table: "UsersTasks",
                column: "TaskRoleId",
                principalTable: "TaskRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UsersTasks",
                type: "bit",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersTasks_TaskRoleId",
                table: "UsersTasks");

            migrationBuilder.DropColumn(
                name: "TaskRoleId",
                table: "UsersTasks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UsersTasks");

            migrationBuilder.DropTable(
                name: "TaskRoles");

        }
    }
}
