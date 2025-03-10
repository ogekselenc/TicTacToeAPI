using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToeAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Player1 = table.Column<string>(type: "TEXT", nullable: false),
                    Player2 = table.Column<string>(type: "TEXT", nullable: false),
                    BoardState = table.Column<string>(type: "TEXT", nullable: true),
                    CurrentPlayer = table.Column<string>(type: "TEXT", nullable: true),
                    IsGameOver = table.Column<bool>(type: "INTEGER", nullable: false),
                    Winner = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
