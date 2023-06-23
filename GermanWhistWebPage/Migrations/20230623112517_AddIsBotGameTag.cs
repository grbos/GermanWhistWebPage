using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GermanWhistWebPage.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBotGameTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBotGame",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBotGame",
                table: "Games");
        }
    }
}
