using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "Players",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AppInstances",
                columns: table => new
                {
                    AppId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppInstances", x => x.AppId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AppId_RoomId",
                table: "Rooms",
                columns: new[] { "AppId", "RoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_AppId_GameRoomEntityId_Name",
                table: "Players",
                columns: new[] { "AppId", "GameRoomEntityId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_AppInstances_AppId",
                table: "Rooms",
                column: "AppId",
                principalTable: "AppInstances",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_AppInstances_AppId",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "AppInstances");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AppId_RoomId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Players_AppId_GameRoomEntityId_Name",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Players");
        }
    }
}
