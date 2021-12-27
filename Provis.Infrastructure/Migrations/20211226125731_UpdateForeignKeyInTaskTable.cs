﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class UpdateForeignKeyInTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskCreatorId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskCreatorId",
                table: "Tasks",
                column: "TaskCreatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskCreatorId",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskCreatorId",
                table: "Tasks",
                column: "TaskCreatorId",
                unique: true);
        }
    }
}
