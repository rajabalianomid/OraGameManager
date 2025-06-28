using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Mafia.Data.Migrations
{
    /// <inheritdoc />
    public partial class GameActionHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionTime",
                table: "GameActionHistories");

            migrationBuilder.AddColumn<string>(
                name: "Phase",
                table: "GameActionHistories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phase",
                table: "GameActionHistories");

            migrationBuilder.AddColumn<int>(
                name: "ActionTime",
                table: "GameActionHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
