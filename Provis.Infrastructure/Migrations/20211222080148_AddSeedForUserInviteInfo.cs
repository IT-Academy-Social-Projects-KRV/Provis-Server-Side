using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class AddSeedForUserInviteInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Workspaces",
                columns: new[] { "Id", "DateOfCreate", "Description", "Name" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "workspace1", "workspace1" });

            migrationBuilder.InsertData(
                table: "Workspaces",
                columns: new[] { "Id", "DateOfCreate", "Description", "Name" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "workspace2", "workspace2" });

            migrationBuilder.InsertData(
                table: "Workspaces",
                columns: new[] { "Id", "DateOfCreate", "Description", "Name" },
                values: new object[] { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "workspace3", "workspace3" });

            migrationBuilder.InsertData(
                table: "InviteUsers",
                columns: new[] { "Id", "Date", "FromUserId", "IsConfirm", "ToUserId", "WorkspaceId" },
                values: new object[] { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "e0baf8de-ed67-42c5-9760-4823dd664816", true, "9c03ecb6-a154-445d-a66b-9cdfa67ff03e", 1 });

            migrationBuilder.InsertData(
                table: "InviteUsers",
                columns: new[] { "Id", "Date", "FromUserId", "IsConfirm", "ToUserId", "WorkspaceId" },
                values: new object[] { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "e0baf8de-ed67-42c5-9760-4823dd664816", false, "baf4ff0f-ea04-443f-80b1-044046a5dc2e", 1 });

            migrationBuilder.InsertData(
                table: "InviteUsers",
                columns: new[] { "Id", "Date", "FromUserId", "IsConfirm", "ToUserId", "WorkspaceId" },
                values: new object[] { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "e0baf8de-ed67-42c5-9760-4823dd664816", null, "baf4ff0f-ea04-443f-80b1-044046a5dc2e", 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "InviteUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "InviteUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "InviteUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Workspaces",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
