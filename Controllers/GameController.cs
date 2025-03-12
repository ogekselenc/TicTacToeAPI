using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.Data;
using TicTacToeAPI.Models;
using System.Text.Json;
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

        // ✅ Start a new game
        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] Game newGame)
        {
            if (newGame.Size < 3 || newGame.WinLength < 3)
                return BadRequest("Board size and win length must be at least 3.");

            newGame.InitializeBoard();
            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
        }

        // ✅ Get game by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        // ✅ Make a move
        [HttpPost("{id}/move")]
        public async Task<IActionResult> MakeMove(int id, [FromBody] Move move)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null || game.IsGameOver)
                return BadRequest("Game not found or already over");

            char[,] board = game.GetBoard();

            // Validate move
            if (move.Row < 0 || move.Row >= game.Size || move.Column < 0 || move.Column >= game.Size || board[move.Row, move.Column] != '-')
                return BadRequest("Invalid move");

            // Make move
            if (string.IsNullOrEmpty(move.Player))
                return BadRequest("Player cannot be null or empty.");

            board[move.Row, move.Column] = move.Player[0];
            game.SetBoard(board);

            // Check for win
            if (CheckWin(board, move.Player[0], game.WinLength))
            {
                game.IsGameOver = true;
                game.Winner = move.Player;
            }

            // Switch player turn
            game.CurrentPlayer = (game.CurrentPlayer == "X") ? "O" : "X";
            await _context.SaveChangesAsync();

            return Ok(game);
        }

        // ✅ Win checking logic (from your original game)
        private bool CheckWin(char[,] board, char mark, int winLength)
        {
            int size = board.GetLength(0);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (CheckDirection(board, i, j, 1, 0, mark, winLength) || // Horizontal
                        CheckDirection(board, i, j, 0, 1, mark, winLength) || // Vertical
                        CheckDirection(board, i, j, 1, 1, mark, winLength) || // Diagonal \
                        CheckDirection(board, i, j, 1, -1, mark, winLength)) // Diagonal /
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckDirection(char[,] board, int row, int col, int rowDir, int colDir, char mark, int winLength)
        {
            int size = board.GetLength(0);
            int count = 0;

            for (int i = 0; i < winLength; i++)
            {
                int newRow = row + i * rowDir;
                int newCol = col + i * colDir;

                if (newRow >= 0 && newRow < size && newCol >= 0 && newCol < size && board[newRow, newCol] == mark)
                {
                    count++;
                }
                else break;
            }

            return count >= winLength;
        }
    }
}
