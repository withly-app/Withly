using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Withly.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixEventRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventInvitee");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_EventInvitee_InviteesId",
                table: "EventInvitee",
                column: "InviteesId");
        }
    }
}
