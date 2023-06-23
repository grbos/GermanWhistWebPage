using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GermanWhistWebPage.Migrations
{
    /// <inheritdoc />
    public partial class IntroduceUserToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_Player1Id",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_Player2Id",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.RenameTable(
                name: "Players",
                newName: "Player");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Player",
                newName: "Discriminator");

            migrationBuilder.AddColumn<string>(
                name: "BotName",
                table: "Player",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Player",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Player",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Player",
                table: "Player",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Player_IdentityUserId",
                table: "Player",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Player_Player1Id",
                table: "Games",
                column: "Player1Id",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Player_Player2Id",
                table: "Games",
                column: "Player2Id",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Player_AspNetUsers_IdentityUserId",
                table: "Player",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Player_Player1Id",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Player_Player2Id",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Player_AspNetUsers_IdentityUserId",
                table: "Player");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Player",
                table: "Player");

            migrationBuilder.DropIndex(
                name: "IX_Player_IdentityUserId",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "BotName",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Player");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Player");

            migrationBuilder.RenameTable(
                name: "Player",
                newName: "Players");

            migrationBuilder.RenameColumn(
                name: "Discriminator",
                table: "Players",
                newName: "Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_Player1Id",
                table: "Games",
                column: "Player1Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_Player2Id",
                table: "Games",
                column: "Player2Id",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
