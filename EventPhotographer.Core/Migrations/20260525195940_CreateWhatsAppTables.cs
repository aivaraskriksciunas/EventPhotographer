using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPhotographer.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateWhatsAppTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Table is dropped but it's okay, there is no real data stored anywhere yeat
            migrationBuilder.DropTable(
                name: "WhatsAppWebhookPayloadLogEntries");

            migrationBuilder.CreateTable(
                name: "WhatsAppContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    WhatsAppId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    WhatsAppUserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProfileName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ActiveParticipantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatsAppContacts_Participants_ActiveParticipantId",
                        column: x => x.ActiveParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WhatsAppWebhookPayloads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppWebhookPayloads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WhatsAppMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    WhatsAppId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WhatsAppContactId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatsAppMessages_WhatsAppContacts_WhatsAppContactId",
                        column: x => x.WhatsAppContactId,
                        principalTable: "WhatsAppContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WhatsAppMedia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    WhatsAppId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Caption = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WhatsAppMessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppMedia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatsAppMedia_WhatsAppMessages_WhatsAppMessageId",
                        column: x => x.WhatsAppMessageId,
                        principalTable: "WhatsAppMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WhatsAppTexts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    Body = table.Column<string>(type: "text", nullable: false),
                    WhatsAppMessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppTexts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatsAppTexts_WhatsAppMessages_WhatsAppMessageId",
                        column: x => x.WhatsAppMessageId,
                        principalTable: "WhatsAppMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppContacts_ActiveParticipantId",
                table: "WhatsAppContacts",
                column: "ActiveParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppContacts_WhatsAppId",
                table: "WhatsAppContacts",
                column: "WhatsAppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppContacts_WhatsAppUserId",
                table: "WhatsAppContacts",
                column: "WhatsAppUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMedia_WhatsAppId",
                table: "WhatsAppMedia",
                column: "WhatsAppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMedia_WhatsAppMessageId",
                table: "WhatsAppMedia",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_WhatsAppContactId",
                table: "WhatsAppMessages",
                column: "WhatsAppContactId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessages_WhatsAppId",
                table: "WhatsAppMessages",
                column: "WhatsAppId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppTexts_WhatsAppMessageId",
                table: "WhatsAppTexts",
                column: "WhatsAppMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppWebhookPayloads_Hash",
                table: "WhatsAppWebhookPayloads",
                column: "Hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatsAppMedia");

            migrationBuilder.DropTable(
                name: "WhatsAppTexts");

            migrationBuilder.DropTable(
                name: "WhatsAppWebhookPayloads");

            migrationBuilder.DropTable(
                name: "WhatsAppMessages");

            migrationBuilder.DropTable(
                name: "WhatsAppContacts");

            migrationBuilder.CreateTable(
                name: "WhatsAppWebhookPayloadLogEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    Hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsValid = table.Column<bool>(type: "boolean", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppWebhookPayloadLogEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppWebhookPayloadLogEntries_Hash",
                table: "WhatsAppWebhookPayloadLogEntries",
                column: "Hash",
                unique: true);
        }
    }
}
