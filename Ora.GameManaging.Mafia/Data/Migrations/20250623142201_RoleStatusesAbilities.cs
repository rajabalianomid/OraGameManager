using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Mafia.Data.Migrations
{
    /// <inheritdoc />
    public partial class RoleStatusesAbilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abilities",
                table: "RoleStatuses");

            migrationBuilder.AddColumn<bool>(
                name: "IsCard",
                table: "Abilities",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RoleStatusesAbilityEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleStatusId = table.Column<int>(type: "int", nullable: false),
                    AbilityId = table.Column<int>(type: "int", nullable: false),
                    AddedLater = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleStatusesAbilityEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleStatusesAbilityEntity_Abilities_AbilityId",
                        column: x => x.AbilityId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleStatusesAbilityEntity_RoleStatuses_RoleStatusId",
                        column: x => x.RoleStatusId,
                        principalTable: "RoleStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleStatusesAbilityEntity_AbilityId",
                table: "RoleStatusesAbilityEntity",
                column: "AbilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleStatusesAbilityEntity_RoleStatusId",
                table: "RoleStatusesAbilityEntity",
                column: "RoleStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleStatusesAbilityEntity");

            migrationBuilder.DropColumn(
                name: "IsCard",
                table: "Abilities");

            migrationBuilder.AddColumn<string>(
                name: "Abilities",
                table: "RoleStatuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
