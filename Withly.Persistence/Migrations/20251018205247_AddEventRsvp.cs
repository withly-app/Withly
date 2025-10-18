using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Withly.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventRsvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitees_Events_EventId",
                table: "Invitees");

            migrationBuilder.DropIndex(
                name: "IX_Invitees_EventId",
                table: "Invitees");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Invitees");

            migrationBuilder.DropColumn(
                name: "RsvpAtUtc",
                table: "Invitees");

            migrationBuilder.DropColumn(
                name: "RsvpStatus",
                table: "Invitees");

            migrationBuilder.CreateTable(
                name: "EventInvitee",
                columns: table => new
                {
                    EventsId = table.Column<Guid>(type: "uuid", nullable: false),
                    InviteesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInvitee", x => new { x.EventsId, x.InviteesId });
                    table.ForeignKey(
                        name: "FK_EventInvitee_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventInvitee_Invitees_InviteesId",
                        column: x => x.InviteesId,
                        principalTable: "Invitees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rsvp",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    InviteeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RespondedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Secret = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rsvp", x => new { x.EventId, x.InviteeId });
                    table.ForeignKey(
                        name: "FK_Rsvp_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rsvp_Invitees_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "Invitees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invitees_Email",
                table: "Invitees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitee_InviteesId",
                table: "EventInvitee",
                column: "InviteesId");

            migrationBuilder.CreateIndex(
                name: "IX_Rsvp_InviteeId",
                table: "Rsvp",
                column: "InviteeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventInvitee");

            migrationBuilder.DropTable(
                name: "Rsvp");

            migrationBuilder.DropIndex(
                name: "IX_Invitees_Email",
                table: "Invitees");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "Invitees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "RsvpAtUtc",
                table: "Invitees",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RsvpStatus",
                table: "Invitees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Invitees_EventId",
                table: "Invitees",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitees_Events_EventId",
                table: "Invitees",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
