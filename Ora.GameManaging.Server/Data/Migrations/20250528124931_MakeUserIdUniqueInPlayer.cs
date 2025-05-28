using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeUserIdUniqueInPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId_GameRoomEntityId",
                table: "Players",
                columns: new[] { "UserId", "GameRoomEntityId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_UserId_GameRoomEntityId",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
