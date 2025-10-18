using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Withly.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EmailAttachmentsMany2Many : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailAttachment_EmailMessages_EmailId",
                table: "EmailAttachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailAttachment",
                table: "EmailAttachment");

            migrationBuilder.DropIndex(
                name: "IX_EmailAttachment_EmailId",
                table: "EmailAttachment");

            migrationBuilder.DropColumn(
                name: "EmailId",
                table: "EmailAttachment");

            migrationBuilder.RenameTable(
                name: "EmailAttachment",
                newName: "EmailAttachments");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailAttachments",
                table: "EmailAttachments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EmailAttachmentEmailMessage",
                columns: table => new
                {
                    AttachmentsId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmailsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAttachmentEmailMessage", x => new { x.AttachmentsId, x.EmailsId });
                    table.ForeignKey(
                        name: "FK_EmailAttachmentEmailMessage_EmailAttachments_AttachmentsId",
                        column: x => x.AttachmentsId,
                        principalTable: "EmailAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailAttachmentEmailMessage_EmailMessages_EmailsId",
                        column: x => x.EmailsId,
                        principalTable: "EmailMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailAttachmentEmailMessage_EmailsId",
                table: "EmailAttachmentEmailMessage",
                column: "EmailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAttachmentEmailMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailAttachments",
                table: "EmailAttachments");

            migrationBuilder.RenameTable(
                name: "EmailAttachments",
                newName: "EmailAttachment");

            migrationBuilder.AddColumn<Guid>(
                name: "EmailId",
                table: "EmailAttachment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailAttachment",
                table: "EmailAttachment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAttachment_EmailId",
                table: "EmailAttachment",
                column: "EmailId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailAttachment_EmailMessages_EmailId",
                table: "EmailAttachment",
                column: "EmailId",
                principalTable: "EmailMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
