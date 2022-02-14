using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class Events : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsGeneral",
                table: "Events",
                newName: "IsCreatorExist");

            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Events",
                newName: "DateOfStart");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfEnd",
                table: "Events",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserEvents",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEvents", x => new { x.UserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_UserEvents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEvents_EventId",
                table: "UserEvents",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEvents");

            migrationBuilder.DropColumn(
                name: "DateOfEnd",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "IsCreatorExist",
                table: "Events",
                newName: "IsGeneral");

            migrationBuilder.RenameColumn(
                name: "DateOfStart",
                table: "Events",
                newName: "DateTime");
        }
    }
}
