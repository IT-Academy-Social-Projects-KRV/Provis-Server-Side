using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class fixForeignKeyInTaskEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_TaskCreaterId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersTasks_Tasks_TasksId",
                table: "UsersTasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskCreaterId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "TasksId",
                table: "UsersTasks",
                newName: "UserTasksId");

            migrationBuilder.RenameColumn(
                name: "TaskCreaterId",
                table: "Tasks",
                newName: "TaskCreatorId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTasks_Tasks_UserTasksId",
                table: "UsersTasks",
                column: "UserTasksId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_TaskCreatorId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersTasks_Tasks_UserTasksId",
                table: "UsersTasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskCreatorId",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "UserTasksId",
                table: "UsersTasks",
                newName: "TasksId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UsersTasks_Tasks_TasksId",
                table: "UsersTasks",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
