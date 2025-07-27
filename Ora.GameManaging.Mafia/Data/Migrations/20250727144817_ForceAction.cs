using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Mafia.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForceAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Force",
                table: "Abilities",
                newName: "PreparingPhase");

            migrationBuilder.AddColumn<bool>(
                name: "ForceAction",
                table: "Abilities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForceAction",
                table: "Abilities");

            migrationBuilder.RenameColumn(
                name: "PreparingPhase",
                table: "Abilities",
                newName: "Force");
        }
    }
}
