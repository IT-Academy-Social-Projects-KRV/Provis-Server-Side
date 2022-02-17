using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class AddIsUsingSprintsFieldInWorkspaceEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isUseSprints",
                table: "Workspaces",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isUseSprints",
                table: "Workspaces");
        }
    }
}
