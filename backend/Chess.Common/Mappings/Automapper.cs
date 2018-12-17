using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using AutoMapper;

namespace Chess.Common.Mappings
{
    public static class Automapper
    {
        public static IMapper GetDefaultMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Game, GameDTO>();
                cfg.CreateMap<GameDTO, Game>();

                cfg.CreateMap<Move, CommitedMoveDTO>()
                .ForMember(p => p.FenBeforeMove, opt => opt.MapFrom(po => po.Fen));
                cfg.CreateMap<CommitedMoveDTO, Move>()
                .ForMember(p => p.Fen, opt => opt.MapFrom(po => po.FenBeforeMove));

                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<UserDTO, User>();

                cfg.CreateMap<Side, SideDTO>();
                cfg.CreateMap<SideDTO, Side>();

            }).CreateMapper();
        }
    }
}
