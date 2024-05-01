using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PearsCleanV3.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPicturesForMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Messages");
        }
    }
}
