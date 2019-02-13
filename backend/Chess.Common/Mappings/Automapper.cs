using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using AutoMapper;
using System.Linq;
using System;

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
                .ForMember(p => p.Moves, opt => opt.Ignore())
                .ForMember(p => p.Sides, opt => opt.Ignore());
                cfg.CreateMap<GameFullDTO, Game>();
                cfg.CreateMap<Game, GameWidthConclusionDTO>()
                .ForMember(p => p.Moves, opt => opt.Ignore())
                .ForMember(p => p.Sides, opt => opt.Ignore())
                .ForMember(p => p.IsDraw, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                {
                    if (src.Status != DataAccess.Helpers.GameStatus.Completed)
                        return false;

                    int? targetUserId = context.Items["ConclusionForUserId"] as int?;
                    if (!targetUserId.HasValue)
                        return false;

                    return src.Sides
                    .Where(s => s.PlayerId == targetUserId.Value)
                    .FirstOrDefault()
                    ?.IsDraw;
                }))
                .ForMember(p => p.IsResigned, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                {
                    if (src.Status != DataAccess.Helpers.GameStatus.Completed)
                        return false;

                    int? targetUserId = context.Items["ConclusionForUserId"] as int?;
                    if (!targetUserId.HasValue)
                        return false;

                    return src.Sides
                    .Where(s => s.PlayerId == targetUserId.Value)
                    .FirstOrDefault()
                    ?.IsResign;
                }))
                .ForMember(p => p.IsWon, opt => opt.ResolveUsing((src, dest, destMember, context) =>
                {
                    if (src.Status != DataAccess.Helpers.GameStatus.Completed)
                        return false;

                    int? targetUserId = context.Items["ConclusionForUserId"] as int?;
                    if (!targetUserId.HasValue || src.Sides.Count() < 2)
                        return false;

                    var first = src.Sides.FirstOrDefault();
                    var second = src.Sides.LastOrDefault();
                    var isFirstIsTarget = first.Id == targetUserId.Value;
                    var isFirstWon = first.Points > second.Points;
                    return ((isFirstWon && isFirstIsTarget) || (!isFirstWon && !isFirstIsTarget));
                }));
                cfg.CreateMap<GameWidthConclusionDTO, Game>();

                cfg.CreateMap<Move, MoveDTO>();
                cfg.CreateMap<MoveDTO, Move>();

                cfg.CreateMap<User, UserDTO>()
                .ForMember(p => p.Uid, opt => opt.UseValue(string.Empty));
                cfg.CreateMap<UserDTO, User>();

                cfg.CreateMap<Side, SideDTO>();
                cfg.CreateMap<SideDTO, Side>();

            }).CreateMapper();
        }
    }
}
