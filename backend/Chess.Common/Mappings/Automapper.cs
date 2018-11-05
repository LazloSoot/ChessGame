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
                cfg.CreateMap<Game, GameDTO>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.Status, opt => opt.MapFrom(po => po.Status))
                .ForMember(p => p.Fen, opt => opt.MapFrom(po => po.Fen));
                cfg.CreateMap<GameDTO, Game>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.Status, opt => opt.MapFrom(po => po.Status))
                .ForMember(p => p.Fen, opt => opt.MapFrom(po => po.Fen));

                cfg.CreateMap<Move, CommitedMoveDTO>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.FenBeforeMove, opt => opt.MapFrom(po => po.Fen))
                .ForMember(p => p.Ply, opt => opt.MapFrom(po => po.Ply))
                .ForMember(p => p.MoveNext, opt => opt.MapFrom(po => po.MoveNext))
                .ForMember(p => p.Game, opt => opt.MapFrom(po => po.Game))
                .ForMember(p => p.GameId, opt => opt.MapFrom(po => po.GameId))
                .ForMember(p => p.Player, opt => opt.MapFrom(po => po.Player))
                .ForMember(p => p.PlayerId, opt => opt.MapFrom(po => po.PlayerId));
                cfg.CreateMap<CommitedMoveDTO, Move>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.Fen, opt => opt.MapFrom(po => po.FenBeforeMove))
                .ForMember(p => p.Ply, opt => opt.MapFrom(po => po.Ply))
                .ForMember(p => p.MoveNext, opt => opt.MapFrom(po => po.MoveNext))
                .ForMember(p => p.Game, opt => opt.MapFrom(po => po.Game))
                .ForMember(p => p.Player, opt => opt.MapFrom(po => po.Player));

                cfg.CreateMap<Player, PlayerDTO>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.Name, opt => opt.MapFrom(po => po.Name));
                cfg.CreateMap<PlayerDTO, Player>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.Name, opt => opt.MapFrom(po => po.Name));

                cfg.CreateMap<Side, SideDTO>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.IsDraw, opt => opt.MapFrom(po => po.IsDraw))
                .ForMember(p => p.IsResign, opt => opt.MapFrom(po => po.IsResign))
                .ForMember(p => p.Player, opt => opt.MapFrom(po => po.Player))
                .ForMember(p => p.Points, opt => opt.MapFrom(po => po.Points))
                .ForMember(p => p.Color, opt => opt.MapFrom(po => po.Color));
                cfg.CreateMap<SideDTO, Side>()
                .ForMember(p => p.Id, opt => opt.MapFrom(po => po.Id))
                .ForMember(p => p.IsDraw, opt => opt.MapFrom(po => po.IsDraw))
                .ForMember(p => p.IsResign, opt => opt.MapFrom(po => po.IsResign))
                .ForMember(p => p.Player, opt => opt.MapFrom(po => po.Player))
                .ForMember(p => p.Points, opt => opt.MapFrom(po => po.Points))
                .ForMember(p => p.Color, opt => opt.MapFrom(po => po.Color));

            }).CreateMapper();
        }
    }
}
