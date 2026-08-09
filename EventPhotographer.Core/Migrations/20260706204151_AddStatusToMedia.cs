using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPhotographer.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Media",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Media");
        }
    }
}
