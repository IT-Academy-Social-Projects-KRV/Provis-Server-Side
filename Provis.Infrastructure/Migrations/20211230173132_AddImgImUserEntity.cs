using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class AddImgImUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "AspNetUsers");
        }
    }
}
