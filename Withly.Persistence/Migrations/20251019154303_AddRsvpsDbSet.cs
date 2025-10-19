using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Withly.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRsvpsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rsvp_Events_EventId",
                table: "Rsvp");

            migrationBuilder.DropForeignKey(
                name: "FK_Rsvp_Invitees_InviteeId",
                table: "Rsvp");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rsvp",
                table: "Rsvp");

            migrationBuilder.RenameTable(
                name: "Rsvp",
                newName: "Rsvps");

            migrationBuilder.RenameIndex(
                name: "IX_Rsvp_InviteeId",
                table: "Rsvps",
                newName: "IX_Rsvps_InviteeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rsvps",
                table: "Rsvps",
                columns: new[] { "EventId", "InviteeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Rsvps_Events_EventId",
                table: "Rsvps",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rsvps_Invitees_InviteeId",
                table: "Rsvps",
                column: "InviteeId",
                principalTable: "Invitees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rsvps_Events_EventId",
                table: "Rsvps");

            migrationBuilder.DropForeignKey(
                name: "FK_Rsvps_Invitees_InviteeId",
                table: "Rsvps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rsvps",
                table: "Rsvps");

            migrationBuilder.RenameTable(
                name: "Rsvps",
                newName: "Rsvp");

            migrationBuilder.RenameIndex(
                name: "IX_Rsvps_InviteeId",
                table: "Rsvp",
                newName: "IX_Rsvp_InviteeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rsvp",
                table: "Rsvp",
                columns: new[] { "EventId", "InviteeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Rsvp_Events_EventId",
                table: "Rsvp",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rsvp_Invitees_InviteeId",
                table: "Rsvp",
                column: "InviteeId",
                principalTable: "Invitees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
