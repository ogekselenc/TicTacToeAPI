using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TicTacToeAPI.Hubs
{
    public class GameHub : Hub
    {
        private readonly ILogger<GameHub> _logger;
        

        // Store waiting players and active games in thread-safe dictionaries
        private static ConcurrentDictionary<string, Player> _waitingPlayers = new ConcurrentDictionary<string, Player>();
        private static ConcurrentDictionary<string, Game> _activeGames = new ConcurrentDictionary<string, Game>();

        public GameHub(ILogger<GameHub> logger)
        {
            _logger = logger;
        }

        // Called when a client connects to the hub
        public override async Task OnConnectedAsync()
        {
            try
            {
                _logger.LogInformation($"Client connected: {Context.ConnectionId}");
                await base.OnConnectedAsync();
                // Send a welcome message to the connected client
                await Clients.Caller.SendAsync("ReceiveMessage", "Connected to the game hub.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during OnConnectedAsync for {Context.ConnectionId}");
            }
        }

        // Called when a client disconnects from the hub
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");

                // Handle player disconnection
                if (_waitingPlayers.TryRemove(Context.ConnectionId, out var player))
                {
                    // Notify the disconnected player that they have been removed from the waiting list
                    await Clients.Caller.SendAsync("ReceiveMessage", "You have been removed from the waiting list.");
                }

                if (_activeGames.TryRemove(Context.ConnectionId, out var game))
                {
                    // Notify the other player that the disconnected player has left the game
                    if (game.Player1?.ConnectionId != Context.ConnectionId && !string.IsNullOrEmpty(game.Player1?.ConnectionId))
                    {
                        await Clients.Client(game.Player1.ConnectionId).SendAsync("ReceiveMessage", $"Player {player?.Name} has left the game.");
                        await Clients.Client(game.Player1.ConnectionId).SendAsync("GameEnded", "Game ended due to player disconnection.");
                    }
                    if (game.Player2?.ConnectionId != Context.ConnectionId && !string.IsNullOrEmpty(game.Player2?.ConnectionId))
                    {
                        await Clients.Client(game.Player2.ConnectionId).SendAsync("ReceiveMessage", $"Player {player?.Name} has left the game.");
                        await Clients.Client(game.Player2.ConnectionId).SendAsync("GameEnded", "Game ended due to player disconnection.");
                    }
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during OnDisconnectedAsync for {Context.ConnectionId}");
            }
        }

        // Method for a player to join the waiting list
        public async Task JoinGame(string playerName)
        {
            try
            {
                _logger.LogInformation($"Player {playerName} joining game from {Context.ConnectionId}");

                // Create a new player object with the connection ID, name, and default mark "X"
                var player = new Player { ConnectionId = Context.ConnectionId, Name = playerName, Mark = "X" };

                if (_waitingPlayers.IsEmpty)
                {
                    // If no players are waiting, add the current player to the waiting list
                    _waitingPlayers[Context.ConnectionId] = player;
                    // Notify the player that they are waiting for another player to join
                    await Clients.Caller.SendAsync("ReceiveMessage", "Waiting for another player to join...");
                }
                else
                {
                    // If there is already a waiting player, start a new game
                    var firstPlayer = _waitingPlayers.Values.First();
                    _waitingPlayers.TryRemove(firstPlayer.ConnectionId, out _);

                    // Create a new game with the two players
                    var game = new Game
                    {
                        Player1 = firstPlayer,
                        Player2 = player
                    };

                    // Add the game to the active games dictionary for both players
                    _activeGames[firstPlayer.ConnectionId] = game;
                    _activeGames[player.ConnectionId] = game;

                    // Add both players to the SignalR group for this game
                    await Groups.AddToGroupAsync(firstPlayer.ConnectionId, game.GameId);
                    await Groups.AddToGroupAsync(player.ConnectionId, game.GameId);

                    // Notify both players that the game has started
                    await Clients.Group(game.GameId).SendAsync("ReceiveMessage", $"Game started between {firstPlayer.Name} (X) and {player.Name} (O).");
                    await Clients.Group(game.GameId).SendAsync("GameStarted", game);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during JoinGame for {Context.ConnectionId}");
            }
        }

        // Method for the first player to set the board size and winning line length
        public async Task SetGameSettings(int boardSize, int winLength)
        {
            try
            {
                _logger.LogInformation($"Setting game settings for {Context.ConnectionId}: BoardSize={boardSize}, WinLength={winLength}");

                // Check if the current player is in an active game
                if (_activeGames.TryGetValue(Context.ConnectionId, out var game))
                {
                    // Update the game settings
                    game.BoardSize = boardSize;
                    game.WinLength = winLength;
                    // Initialize the game board with the new size
                    game.InitializeBoard();
                    // Notify both players of the updated game settings
                    await Clients.Group(game.GameId).SendAsync("ReceiveMessage", $"Board size set to {boardSize}x{boardSize}, winning line length set to {winLength}.");
                    await Clients.Group(game.GameId).SendAsync("GameSettingsUpdated", game);
                }
                else
                {
                    _logger.LogWarning($"No active game found for {Context.ConnectionId}");
                    await Clients.Caller.SendAsync("ReceiveMessage", "No active game found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during SetGameSettings for {Context.ConnectionId}");
            }
        }

        // Method for a player to make a move
        public async Task MakeMove(int row, int col)
        {
            try
            {
                _logger.LogInformation($"Making move for {Context.ConnectionId}: Row={row}, Col={col}");

                // Check if the current player is in an active game
                if (_activeGames.TryGetValue(Context.ConnectionId, out var game))
                {
                    // Determine which player is making the move
                    var player = game.Player1.ConnectionId == Context.ConnectionId ? game.Player1 : game.Player2;

                    // Attempt to make the move on the game board
                    var moveResult = game.MakeMove(row, col, player.Mark[0]); // Use the first character of the mark

                    if (moveResult.IsValid)
                    {
                        // If the move is valid, notify both players of the updated game state
                        await Clients.Group(game.GameId).SendAsync("MoveMade", game);

                        if (moveResult.IsWin)
                        {
                            // If the move results in a win, notify both players and end the game
                            await Clients.Group(game.GameId).SendAsync("ReceiveMessage", $"Player {player.Name} wins!");
                            await Clients.Group(game.GameId).SendAsync("GameEnded", $"Player {player.Name} wins!");
                            // Remove the game from the active games dictionary
                            _activeGames.TryRemove(game.Player1.ConnectionId, out _);
                            _activeGames.TryRemove(game.Player2.ConnectionId, out _);
                        }
                        else if (moveResult.IsDraw)
                        {
                            // If the move results in a draw, notify both players and end the game
                            await Clients.Group(game.GameId).SendAsync("ReceiveMessage", "The game is a draw.");
                            await Clients.Group(game.GameId).SendAsync("GameEnded", "The game is a draw.");
                            // Remove the game from the active games dictionary
                            _activeGames.TryRemove(game.Player1.ConnectionId, out _);
                            _activeGames.TryRemove(game.Player2.ConnectionId, out _);
                        }
                    }
                    else
                    {
                        // If the move is invalid, notify the player
                        await Clients.Caller.SendAsync("ReceiveMessage", "Invalid move.");
                    }
                }
                else
                {
                    _logger.LogWarning($"No active game found for {Context.ConnectionId}");
                    await Clients.Caller.SendAsync("ReceiveMessage", "No active game found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during MakeMove for {Context.ConnectionId}");
            }
        }

        // Class representing a player
        public class Player
        {
            public string ConnectionId { get; set; } = string.Empty; // Unique connection ID of the player
            public string Name { get; set; } = string.Empty; // Name of the player
            public string Mark { get; set; } = string.Empty; // Mark used by the player (X or O)
        }

        // Class representing a game
        public class Game
        {
            public string GameId { get; set; } = Guid.NewGuid().ToString(); // Unique ID for the game
            public Player Player1 { get; set; } = new Player(); // First player
            public Player Player2 { get; set; } = new Player(); // Second player
            public int BoardSize { get; set; } = 3; // Size of the game board (default is 3x3)
            public int WinLength { get; set; } = 3; // Length of the winning line (default is 3)
            public List<List<char>> Board { get; set; } = new List<List<char>>(); // 2D list representing the game board

            // Method to initialize the game board with empty spaces
            public void InitializeBoard()
            {
                Board = new List<List<char>>();
                for (int i = 0; i < BoardSize; i++)
                {
                    var row = new List<char>();
                    for (int j = 0; j < BoardSize; j++)
                    {
                        row.Add(' '); // Add an empty space to each cell
                    }
                    Board.Add(row);
                }
            }

            // Method to make a move on the game board
            public MoveResult MakeMove(int row, int col, char mark)
            {
                // Check if the move is within the bounds of the board and the cell is empty
                if (row < 0 || row >= BoardSize || col < 0 || col >= BoardSize || Board[row][col] != ' ')
                {
                    return new MoveResult { IsValid = false }; // Return invalid move
                }

                // Place the player's mark on the board
                Board[row][col] = mark;

                // Check if the move results in a win
                if (CheckWin(row, col, mark))
                {
                    return new MoveResult { IsValid = true, IsWin = true }; // Return valid move with win
                }

                // Check if the move results in a draw
                if (CheckDraw())
                {
                    return new MoveResult { IsValid = true, IsDraw = true }; // Return valid move with draw
                }

                // Return valid move (no win or draw)
                return new MoveResult { IsValid = true };
            }

            // Method to check if the move results in a win
            private bool CheckWin(int row, int col, char mark)
            {
                // Implement win checking logic (similar to your existing logic)
                // Check horizontal, vertical, and diagonals for a winning line of length WinLength
                return false;
            }

            // Method to check if the game is a draw
            private bool CheckDraw()
            {
                // Check if the board is full
                return Board.All(row => row.All(cell => cell != ' '));
            }
        }

        // Class representing the result of a move
        public class MoveResult
        {
            public bool IsValid { get; set; } // Whether the move is valid
            public bool IsWin { get; set; } // Whether the move results in a win
            public bool IsDraw { get; set; } // Whether the move results in a draw
        }
    }
}
