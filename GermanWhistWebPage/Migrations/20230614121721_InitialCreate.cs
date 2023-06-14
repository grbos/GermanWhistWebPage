using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GermanWhistWebPage.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Player1Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Player2Id = table.Column<int>(type: "INTEGER", nullable: false),
                    CardStack = table.Column<string>(type: "TEXT", nullable: false),
                    HandPlayer1 = table.Column<string>(type: "TEXT", nullable: false),
                    HandPlayer2 = table.Column<string>(type: "TEXT", nullable: false),
                    NewHandCardIdPlayer1 = table.Column<int>(type: "INTEGER", nullable: true),
                    NewHandCardIdPlayer2 = table.Column<int>(type: "INTEGER", nullable: true),
                    PlayedCardIdPlayer1 = table.Column<int>(type: "INTEGER", nullable: true),
                    PlayedCardIdPlayer2 = table.Column<int>(type: "INTEGER", nullable: true),
                    StartingPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrickStartPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentPlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrumpSuit = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetScore = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalScorePlayer1 = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalScorePlayer2 = table.Column<int>(type: "INTEGER", nullable: false),
                    RoundScorePlayer1 = table.Column<int>(type: "INTEGER", nullable: false),
                    RoundScorePlayer2 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Players_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Players_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player1Id",
                table: "Games",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player2Id",
                table: "Games",
                column: "Player2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
