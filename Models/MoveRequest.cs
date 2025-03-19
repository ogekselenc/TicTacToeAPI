namespace TicTacToeAPI.Models
{
    public class MoveRequest
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public string Player { get; set; } = string.Empty;
    }
}
