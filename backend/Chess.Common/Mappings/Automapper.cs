using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using AutoMapper;
using System.Linq;

namespace Chess.Common.Mappings
{
    public static class Automapper
    {
        public static IMapper GetDefaultMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Game, GameDTO>();
                cfg.CreateMap<GameDTO, Game>()
                .ForMember(p => p.Moves, opt => opt.MapFrom(po => po.Moves.ToList()))
                .ForMember(p => p.Sides, opt => opt.MapFrom(po => po.Sides.ToList()));

                cfg.CreateMap<Move, MoveDTO>()
                .ForMember(p => p.FenBeforeMove, opt => opt.MapFrom(po => po.Fen));
                cfg.CreateMap<MoveDTO, Move>()
                .ForMember(p => p.Fen, opt => opt.MapFrom(po => po.FenBeforeMove));

                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<UserDTO, User>();

                cfg.CreateMap<Side, SideDTO>();
                cfg.CreateMap<SideDTO, Side>();

            }).CreateMapper();
        }
    }
}
