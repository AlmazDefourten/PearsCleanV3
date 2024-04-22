using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PearsCleanV3.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SwipedUserId",
                table: "Matches",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_SwipedUserId",
                table: "Matches",
                column: "SwipedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_AspNetUsers_SwipedUserId",
                table: "Matches",
                column: "SwipedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_AspNetUsers_SwipedUserId",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_SwipedUserId",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "SwipedUserId",
                table: "Matches");
        }
    }
}
