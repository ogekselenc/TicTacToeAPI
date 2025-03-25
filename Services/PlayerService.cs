using AutoMapper;
using TicTacToeAPI.DTOs;
using TicTacToeAPI.Interfaces;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Services;

public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PlayerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PlayerDTO> CreatePlayerAsync(CreatePlayerDTO createPlayerDto)
    {
        var player = new Player
        {
            Name = createPlayerDto.Name,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Players.AddAsync(player);
        await _unitOfWork.CompleteAsync();

        return _mapper.Map<PlayerDTO>(player);
    }

    public async Task<PlayerDTO> GetPlayerAsync(int id)
    {
        var player = await _unitOfWork.Players.GetByIdAsync(id);
        if (player == null)
            throw new KeyNotFoundException("Player not found");

        return _mapper.Map<PlayerDTO>(player);
    }

    public async Task<IEnumerable<PlayerDTO>> GetPlayersAsync()
    {
        var players = await _unitOfWork.Players.GetAllAsync();
        return _mapper.Map<IEnumerable<PlayerDTO>>(players);
    }
}