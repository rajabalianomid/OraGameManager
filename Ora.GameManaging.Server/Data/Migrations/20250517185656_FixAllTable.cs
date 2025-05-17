using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixAllTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastSnapshotJson",
                table: "Rooms",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "Events",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_GameRoomEntityId",
                table: "Events",
                column: "GameRoomEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Rooms_GameRoomEntityId",
                table: "Events",
                column: "GameRoomEntityId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Rooms_GameRoomEntityId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_GameRoomEntityId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LastSnapshotJson",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerName",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
