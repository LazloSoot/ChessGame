using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using AutoMapper;
using System.Linq;
using System;
using Chess.DataAccess.Helpers;

namespace Chess.Common.Mappings
{
    public static class Automapper
    {
        public static IMapper GetDefaultMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Game, GameFullDTO>();
                cfg.CreateMap<GameFullDTO, Game>()
                .ForMember(p => p.Moves, opt => opt.MapFrom(po => po.Moves.ToList()))
                .ForMember(p => p.Sides, opt => opt.MapFrom(po => po.Sides.ToList()));
                cfg.CreateMap<Game, GamePartialDTO>()
                .ForMember(p => p.Moves, opt => opt.Ignore());
                cfg.CreateMap<GameFullDTO, Game>();
                cfg.CreateMap<Move, MoveDTO>();
                cfg.CreateMap<MoveDTO, Move>();

                cfg.CreateMap<User, UserDTO>()
                .ForMember(p => p.Uid, opt => opt.UseValue(string.Empty));
                cfg.CreateMap<UserDTO, User>();

                cfg.CreateMap<Side, SideDTO>();
                cfg.CreateMap<SideDTO, Side>();

                cfg.CreateMap(typeof(PagedResult<>), typeof(PagedResultDTO<>));

            }).CreateMapper();
        }
    }
}
