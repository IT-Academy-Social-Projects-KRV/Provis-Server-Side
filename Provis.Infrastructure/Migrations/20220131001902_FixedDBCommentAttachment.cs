using Microsoft.EntityFrameworkCore.Migrations;

namespace Provis.Infrastructure.Migrations
{
    public partial class FixedDBCommentAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentAttachment_Comments_CommentId",
                table: "CommentAttachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentAttachment",
                table: "CommentAttachment");

            migrationBuilder.RenameTable(
                name: "CommentAttachment",
                newName: "CommentAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_CommentAttachment_CommentId",
                table: "CommentAttachments",
                newName: "IX_CommentAttachments_CommentId");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentPath",
                table: "CommentAttachments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentAttachments",
                table: "CommentAttachments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentAttachments_Comments_CommentId",
                table: "CommentAttachments",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentAttachments_Comments_CommentId",
                table: "CommentAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CommentAttachments",
                table: "CommentAttachments");

            migrationBuilder.RenameTable(
                name: "CommentAttachments",
                newName: "CommentAttachment");

            migrationBuilder.RenameIndex(
                name: "IX_CommentAttachments_CommentId",
                table: "CommentAttachment",
                newName: "IX_CommentAttachment_CommentId");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentPath",
                table: "CommentAttachment",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommentAttachment",
                table: "CommentAttachment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentAttachment_Comments_CommentId",
                table: "CommentAttachment",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
