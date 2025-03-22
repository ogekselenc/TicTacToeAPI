using Microsoft.AspNetCore.SignalR;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinGame(int gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Game-{gameId}");
            await Clients.Group($"Game-{gameId}").SendAsync("PlayerJoined", gameId);
        }

        public async Task SendMove(int gameId, Move move)
        {
            await Clients.Group($"Game-{gameId}").SendAsync("ReceiveMove", move);
        }

        public async Task NotifyGameOver(int gameId, string outcome)
        {
            await Clients.Group($"Game-{gameId}").SendAsync("GameOver", outcome);
        }
    }
}