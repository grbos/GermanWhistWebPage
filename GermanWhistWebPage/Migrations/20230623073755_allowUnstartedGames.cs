using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GermanWhistWebPage.Migrations
{
    /// <inheritdoc />
    public partial class allowUnstartedGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Player_Player2Id",
                table: "Games");

            migrationBuilder.AlterColumn<int>(
                name: "Player2Id",
                table: "Games",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Player_Player2Id",
                table: "Games",
                column: "Player2Id",
                principalTable: "Player",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Player_Player2Id",
                table: "Games");

            migrationBuilder.AlterColumn<int>(
                name: "Player2Id",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Player_Player2Id",
                table: "Games",
                column: "Player2Id",
                principalTable: "Player",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
