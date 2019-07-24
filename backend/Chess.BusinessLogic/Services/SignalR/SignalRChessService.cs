using Chess.BusinessLogic.Hubs;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Chess.Common.Helpers;

namespace Chess.BusinessLogic.Services.SignalR
{
    public class SignalRChessService : SignalRAbsService<ChessGameHub>, ISignalRChessService
    {
        public SignalRChessService(IHubContext<ChessGameHub> hubContext, ICurrentUserProvider currentUserProvider)
            : base(hubContext, currentUserProvider)
        {

        }

        public async Task CommitMove(int gameId)
        {
            await _hubContext
                .Clients
                .Group($"{HubGroup.Game.GetStringValue()}{gameId}")
                .SendAsync(ClientEvent.MoveCommitted.GetStringValue());
        }

        public async Task EmitMate(int gameId, Color mateTo)
        {
            await _hubContext
                .Clients
                .Group($"{HubGroup.Game.GetStringValue()}{gameId}")
                .SendAsync(ClientEvent.Mate.GetStringValue(), mateTo == Color.White ? 1 : 2);
        }

        public async Task EmitResign(int gameId, Color resignedSide)
        {
            await _hubContext
                .Clients
                .Group($"{HubGroup.Game.GetStringValue()}{gameId}")
                .SendAsync(ClientEvent.Resign.GetStringValue(), resignedSide);
        }


        public async Task EmitDraw(int gameId, Color side)
        {
            await _hubContext
                .Clients
                .Groups($"{HubGroup.Game.GetStringValue()}{gameId}")
                .SendAsync(ClientEvent.Draw.GetStringValue(), side);
        }

        public async Task EmitСheck(int gameId, Color checkTo)
        {
            await _hubContext
                .Clients
                .Group($"{HubGroup.Game.GetStringValue()}{gameId}")
                .SendAsync(ClientEvent.Check.GetStringValue(), checkTo == Color.White ? 1 : 2);
        }
    }
}
