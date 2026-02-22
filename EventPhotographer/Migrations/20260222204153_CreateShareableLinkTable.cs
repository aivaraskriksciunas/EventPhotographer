using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPhotographer.Migrations
{
    /// <inheritdoc />
    public partial class CreateShareableLinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventShareableLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuidv7()"),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventShareableLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventShareableLinks_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventShareableLinks_Code",
                table: "EventShareableLinks",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventShareableLinks_EventId",
                table: "EventShareableLinks",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventShareableLinks");
        }
    }
}
