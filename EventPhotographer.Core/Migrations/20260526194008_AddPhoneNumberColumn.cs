using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPhotographer.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneNumberColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "WhatsAppMessages",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "WhatsAppMessages");
        }
    }
}
