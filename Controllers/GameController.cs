using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Data;
using TicTacToeAPI.Models;
using System.Threading.Tasks;

namespace TicTacToeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly TicTacToeDbContext _context;

        public GameController(TicTacToeDbContext context)
        {
            _context = context;
        }

        // Start a new game
        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] Game newGame)
        {
            newGame.BoardState = "---------"; // Empty 3x3 board
            newGame.CurrentPlayer = "X";
            newGame.IsGameOver = false;
            newGame.Winner = null;

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
        }

        // Get game by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        // Make a move
        [HttpPost("{id}/move")]
        public async Task<IActionResult> MakeMove(int id, [FromBody] Move move)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null || game.IsGameOver)
                return BadRequest("Game not found or already over");

            // Convert BoardState string to char array
            char[] board = game.BoardState?.ToCharArray() ?? new char[9];

            int index = move.Row * 3 + move.Column;

            if (index < 0 || index >= board.Length || board[index] != '-')
                return BadRequest("Invalid move");

            // Make the move
            board[index] = move.Player[0];

            // ✅ Use helper function to convert back to string
            game.BoardState = ConvertBoardToString(board);

            // Check for win
            if (CheckWin(board, move.Player[0]))
            {
                game.IsGameOver = true;
                game.Winner = move.Player;
            }

            // Switch turn
            game.CurrentPlayer = (game.CurrentPlayer == "X") ? "O" : "X";

            await _context.SaveChangesAsync();

            return Ok(game);
        }

        // ✅ Helper method to convert char array to string safely
        private string ConvertBoardToString(char[] board)
        {
            return new string(board);
        }



        private bool CheckWin(char[] board, char mark)
        {
            string winPattern = new string(mark, 3);

            string[] rows = { new string(board[0..3]), new string(board[3..6]), new string(board[6..9]) };
            string[] cols = { $"{board[0]}{board[3]}{board[6]}", $"{board[1]}{board[4]}{board[7]}", $"{board[2]}{board[5]}{board[8]}" };
            string[] diags = { $"{board[0]}{board[4]}{board[8]}", $"{board[2]}{board[4]}{board[6]}" };

            return rows.Concat(cols).Concat(diags).Any(line => line == winPattern);
        }
    }
}
