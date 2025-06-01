using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Mafia.Data.Migrations
{
    /// <inheritdoc />
    public partial class addability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Abilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Expression = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsNightAbility = table.Column<bool>(type: "bit", nullable: false),
                    IsDayAbility = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abilities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameActionHistories_AbilityId",
                table: "GameActionHistories",
                column: "AbilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameActionHistories_Abilities_AbilityId",
                table: "GameActionHistories",
                column: "AbilityId",
                principalTable: "Abilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameActionHistories_Abilities_AbilityId",
                table: "GameActionHistories");

            migrationBuilder.DropTable(
                name: "Abilities");

            migrationBuilder.DropIndex(
                name: "IX_GameActionHistories_AbilityId",
                table: "GameActionHistories");
        }
    }
}
