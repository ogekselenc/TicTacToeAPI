using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicTacToeAPI.DTOs;
using TicTacToeAPI.Interfaces;
using TicTacToeAPI.Models;


namespace TicTacToeAPI.Services;

public class GameService : IGameService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GameService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GameDTO> CreateGameAsync(CreateGameDTO createGameDto)
    {
        // Validate players exist
        var playerX = await _unitOfWork.Players.GetByIdAsync(createGameDto.PlayerXId);
        if (playerX == null)
            throw new KeyNotFoundException("Player X not found");

        Player playerO = null;
        if (createGameDto.PlayerOId.HasValue)
        {
            playerO = await _unitOfWork.Players.GetByIdAsync(createGameDto.PlayerOId.Value);
            if (playerO == null)
                throw new KeyNotFoundException("Player O not found");
        }

        // Validate board size and winning line
        if (createGameDto.WinningLine > createGameDto.BoardSize)
            throw new ArgumentException("Winning line cannot be larger than board size");

        // Create new game
        var game = new Game
        {
            PlayerXid = createGameDto.PlayerXId,
            PlayerOid = createGameDto.PlayerOId,
            BoardSize = createGameDto.BoardSize,
            WinningLine = createGameDto.WinningLine,
            OutcomeStatus = "InProgress",
            OutcomeReason = "Game created",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Games.AddAsync(game);
        await _unitOfWork.CompleteAsync();

        return await GetGameDTO(game.Id);
    }

    public async Task<GameDTO> JoinGameAsync(int gameId, JoinGameDTO joinGameDto)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(gameId);
        if (game == null)
            throw new KeyNotFoundException("Game not found");

        if (game.PlayerOid != null)
            throw new InvalidOperationException("Game already has two players");

        var playerO = await _unitOfWork.Players.GetByIdAsync(joinGameDto.PlayerOId);
        if (playerO == null)
            throw new KeyNotFoundException("Player O not found");

        if (game.PlayerXid == joinGameDto.PlayerOId)
            throw new InvalidOperationException("Player cannot play against themselves");

        game.PlayerOid = joinGameDto.PlayerOId;
        game.OutcomeReason = "Player O joined";

        _unitOfWork.Games.Update(game);
        await _unitOfWork.CompleteAsync();

        return await GetGameDTO(game.Id);
    }

    public async Task<GameDTO> MakeMoveAsync(int gameId, MakeMoveDTO makeMoveDto)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(gameId);
        if (game == null)
            throw new KeyNotFoundException("Game not found");

        // Validate game state
        if (game.OutcomeStatus != "InProgress")
            throw new InvalidOperationException("Game is not in progress");

        if (game.PlayerOid == null)
            throw new InvalidOperationException("Waiting for second player to join");

        // Validate player
        if (makeMoveDto.PlayerId != game.PlayerXid && makeMoveDto.PlayerId != game.PlayerOid)
            throw new UnauthorizedAccessException("Player is not part of this game");

        // Get all moves to determine whose turn it is
        var moves = await _unitOfWork.Moves.FindAsync(m => m.GameId == gameId);
        var lastMove = moves.OrderByDescending(m => m.CreatedAt).FirstOrDefault();

        // Validate turn order
        if (lastMove != null && lastMove.PlayerId == makeMoveDto.PlayerId)
            throw new InvalidOperationException("It's not your turn");

        // Validate position
        if (makeMoveDto.PositionX < 0 || makeMoveDto.PositionX >= game.BoardSize ||
            makeMoveDto.PositionY < 0 || makeMoveDto.PositionY >= game.BoardSize)
            throw new ArgumentException("Move is outside the board");

        // Check if position is already taken
        if (moves.Any(m => m.PositionX == makeMoveDto.PositionX && m.PositionY == makeMoveDto.PositionY))
            throw new InvalidOperationException("Position already taken");

        // Create the move
        var move = new Move
        {
            GameId = gameId,
            PlayerId = makeMoveDto.PlayerId,
            PositionX = makeMoveDto.PositionX,
            PositionY = makeMoveDto.PositionY,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Moves.AddAsync(move);

        // Check for win or draw
        await CheckGameStatus(game, moves.Append(move).ToList());

        await _unitOfWork.CompleteAsync();

        return await GetGameDTO(game.Id);
    }

    private async Task CheckGameStatus(Game game, List<Move> moves)
    {
        // Get the last move
        var lastMove = moves.Last();
        var playerMoves = moves.Where(m => m.PlayerId == lastMove.PlayerId).ToList();

        // Check for win
        if (CheckForWin(playerMoves, game.WinningLine, game.BoardSize))
        {
            game.OutcomeStatus = "Completed";
            game.OutcomeReason = $"Player {lastMove.PlayerId} wins";
            game.PlayerId = lastMove.PlayerId; // Set the winner
            _unitOfWork.Games.Update(game);
            return;
        }

        // Check for draw
        if (moves.Count == game.BoardSize * game.BoardSize)
        {
            game.OutcomeStatus = "Completed";
            game.OutcomeReason = "Game ended in a draw";
            _unitOfWork.Games.Update(game);
            return;
        }

        // If no win or draw, game continues
        game.OutcomeReason = $"Player {(lastMove.PlayerId == game.PlayerXid ? "O" : "X")}'s turn";
        _unitOfWork.Games.Update(game);
    }


    private bool CheckForWin(List<Move> playerMoves, int winningLine, int boardSize)
    {
        var lastMove = playerMoves.Last();

        // Convert moves to a 2D array for easier checking
        var board = new bool[boardSize, boardSize];
        foreach (var move in playerMoves)
        {
            board[move.PositionX, move.PositionY] = true;
        }

        // Check horizontal
        if (CountConsecutive(board, lastMove.PositionX, lastMove.PositionY, 1, 0) >= winningLine)
            return true;

        // Check vertical
        if (CountConsecutive(board, lastMove.PositionX, lastMove.PositionY, 0, 1) >= winningLine)
            return true;

        // Check diagonal (top-left to bottom-right)
        if (CountConsecutive(board, lastMove.PositionX, lastMove.PositionY, 1, 1) >= winningLine)
            return true;

        // Check diagonal (top-right to bottom-left)
        if (CountConsecutive(board, lastMove.PositionX, lastMove.PositionY, 1, -1) >= winningLine)
            return true;

        return false;
    }

    private int CountConsecutive(bool[,] board, int startX, int startY, int dx, int dy)
    {
        int count = 1; // Count the starting position
        int boardSize = board.GetLength(0);

        // Check in positive direction
        for (int i = 1; i < boardSize; i++)
        {
            int x = startX + i * dx;
            int y = startY + i * dy;

            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize || !board[x, y])
                break;

            count++;
        }

        // Check in negative direction
        for (int i = 1; i < boardSize; i++)
        {
            int x = startX - i * dx;
            int y = startY - i * dy;

            if (x < 0 || x >= boardSize || y < 0 || y >= boardSize || !board[x, y])
                break;

            count++;
        }

        return count;
    }

    public async Task<GameDTO> GetGameAsync(int gameId)
    {
        return await GetGameDTO(gameId);
    }

    public async Task<IEnumerable<GameDTO>> GetActiveGamesAsync()
    {
        var games = await _unitOfWork.Games.FindAsync(g => g.OutcomeStatus == "InProgress");
        var gameDTOs = new List<GameDTO>();

        foreach (var game in games)
        {
            gameDTOs.Add(await GetGameDTO(game.Id));
        }

        return gameDTOs;
    }

    public async Task<MoveDTO> GetMoveAsync(int moveId)
    {
        var move = await _unitOfWork.Moves.GetByIdAsync(moveId);
        if (move == null)
            throw new KeyNotFoundException("Move not found");

        var player = await _unitOfWork.Players.GetByIdAsync(move.PlayerId);

        return new MoveDTO
        {
            Id = move.Id,
            PlayerId = move.PlayerId,
            PlayerName = player?.Name,
            PositionX = move.PositionX,
            PositionY = move.PositionY,
            CreatedAt = move.CreatedAt
        };
    }

    public async Task<IEnumerable<MoveDTO>> GetGameMovesAsync(int gameId)
    {
        var moves = await _unitOfWork.Moves.FindAsync(m => m.GameId == gameId);
        var moveDTOs = new List<MoveDTO>();

        foreach (var move in moves)
        {
            var player = await _unitOfWork.Players.GetByIdAsync(move.PlayerId);
            moveDTOs.Add(new MoveDTO
            {
                Id = move.Id,
                PlayerId = move.PlayerId,
                PlayerName = player?.Name,
                PositionX = move.PositionX,
                PositionY = move.PositionY,
                CreatedAt = move.CreatedAt
            });
        }

        return moveDTOs.OrderBy(m => m.CreatedAt);
    }

    private async Task<GameDTO> GetGameDTO(int gameId)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(gameId);
        if (game == null)
            throw new KeyNotFoundException("Game not found");

        var playerX = await _unitOfWork.Players.GetByIdAsync(game.PlayerXid);
        var playerO = game.PlayerOid.HasValue ? await _unitOfWork.Players.GetByIdAsync(game.PlayerOid.Value) : null;
        var moves = await _unitOfWork.Moves.FindAsync(m => m.GameId == gameId);

        var moveDTOs = moves.Select(m => new MoveDTO
        {
            Id = m.Id,
            PlayerId = m.PlayerId,
            PlayerName = m.PlayerId == game.PlayerXid ? playerX.Name : playerO?.Name,
            PositionX = m.PositionX,
            PositionY = m.PositionY,
            CreatedAt = m.CreatedAt
        }).OrderBy(m => m.CreatedAt).ToList();

        return new GameDTO
        {
            Id = game.Id,
            PlayerX = _mapper.Map<PlayerDTO>(playerX),
            PlayerO = playerO != null ? _mapper.Map<PlayerDTO>(playerO) : null,
            BoardSize = game.BoardSize,
            WinningLine = game.WinningLine,
            Status = game.OutcomeStatus,
            CreatedAt = game.CreatedAt,
            Moves = moveDTOs
        };
    }
    public async Task<GameDTO> ResignGameAsync(int gameId, int playerId)
    {
        var game = await _unitOfWork.Games.GetByIdAsync(gameId);
        if (game == null)
            throw new KeyNotFoundException("Game not found");

        if (game.OutcomeStatus != "InProgress")
            throw new InvalidOperationException("Game is not in progress");

        if (playerId != game.PlayerXid && playerId != game.PlayerOid)
            throw new UnauthorizedAccessException("Player is not part of this game");

        game.OutcomeStatus = "Completed";
        game.OutcomeReason = $"Player {playerId} resigned";
        game.PlayerId = playerId == game.PlayerXid ? game.PlayerOid : game.PlayerXid; // Opponent wins

        _unitOfWork.Games.Update(game);
        await _unitOfWork.CompleteAsync();

        return await GetGameDTO(game.Id);
    }
}