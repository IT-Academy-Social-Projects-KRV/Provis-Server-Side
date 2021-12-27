using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class seed_data_for_task_statuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_TaskCreaterId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskCreaterId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TaskCreaterId",
                table: "Tasks",
                newName: "TaskCreatorId");

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "StatusName" },
                values: new object[,]
                {
                    { 1, "To do" },
                    { 2, "In progress" },
                    { 3, "In review" },
                    { 4, "Compleated" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskCreatorId",
                table: "Tasks",
                column: "TaskCreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_TaskCreatorId",
                table: "Tasks",
                column: "TaskCreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_TaskCreatorId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskCreatorId",
                table: "Tasks");

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.RenameColumn(
                name: "TaskCreatorId",
                table: "Tasks",
                newName: "TaskCreaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskCreaterId",
                table: "Tasks",
                column: "TaskCreaterId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_TaskCreaterId",
                table: "Tasks",
                column: "TaskCreaterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
