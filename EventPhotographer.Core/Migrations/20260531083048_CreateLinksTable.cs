using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPhotographer.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateLinksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WhatsAppMessageLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DeepLinkUrl = table.Column<string>(type: "text", nullable: true),
                    PrefilledMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShareableLinkId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsAppMessageLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WhatsAppMessageLinks_EventShareableLinks_ShareableLinkId",
                        column: x => x.ShareableLinkId,
                        principalTable: "EventShareableLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessageLinks_Code",
                table: "WhatsAppMessageLinks",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WhatsAppMessageLinks_ShareableLinkId",
                table: "WhatsAppMessageLinks",
                column: "ShareableLinkId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatsAppMessageLinks");
        }
    }
}
