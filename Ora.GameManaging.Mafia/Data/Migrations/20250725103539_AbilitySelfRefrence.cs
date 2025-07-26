using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ora.GameManaging.Mafia.Data.Migrations
{
    /// <inheritdoc />
    public partial class AbilitySelfRefrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Abilities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pattern",
                table: "Abilities",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Abilities_ParentId",
                table: "Abilities",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Abilities_Abilities_ParentId",
                table: "Abilities",
                column: "ParentId",
                principalTable: "Abilities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Abilities_Abilities_ParentId",
                table: "Abilities");

            migrationBuilder.DropIndex(
                name: "IX_Abilities_ParentId",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Abilities");

            migrationBuilder.DropColumn(
                name: "Pattern",
                table: "Abilities");
        }
    }
}
