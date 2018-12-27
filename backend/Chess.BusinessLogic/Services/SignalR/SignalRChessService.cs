using Chess.BusinessLogic.Hubs;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Chess.BusinessLogic.Services.SignalR
{
    public class SignalRChessService : SignalRAbsService<ChessGameHub>, ISignalRChessService
    {
        public SignalRChessService(IHubContext<ChessGameHub> hubContext, ICurrentUser currentUserProvider)
            : base(hubContext, currentUserProvider)
        {

        }
    }
}
