using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Mafia.Data.Migrations
{
    /// <inheritdoc />
    public partial class AbilityForceAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Force",
                table: "Abilities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Force",
                table: "Abilities");
        }
    }
}
