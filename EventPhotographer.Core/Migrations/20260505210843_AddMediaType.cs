using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventPhotographer.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMediaType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UploadToken",
                table: "Media",
                type: "uuid",
                nullable: true,
                defaultValueSql: "uuidv4()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Media",
                type: "character varying(25)",
                maxLength: 25,
                nullable: false,
                defaultValue: "UserUpload");

            migrationBuilder.CreateIndex(
                name: "IX_Media_UploadToken",
                table: "Media",
                column: "UploadToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Media_UploadToken",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Media");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadToken",
                table: "Media",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true,
                oldDefaultValueSql: "uuidv4()");
        }
    }
}
