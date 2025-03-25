using AutoMapper;
using TicTacToeAPI.DTOs;
using TicTacToeAPI.Models;

namespace TicTacToeAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Player, PlayerDTO>();
        CreateMap<CreatePlayerDTO, Player>();
    }
}