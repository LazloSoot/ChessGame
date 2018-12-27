using Chess.BusinessLogic.Helpers.SignalR;
using Chess.BusinessLogic.Hubs;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.Helpers;
using Chess.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Action = Chess.Common.Helpers.Action;

namespace Chess.BusinessLogic.Services.SignalR
{
    public class SignalRNotificationService : SignalRAbsService<NotificationHub>, ISignalRNotificationService
    {
        public SignalRNotificationService(IHubContext<NotificationHub> hubContext, ICurrentUser currentUserProvider)
            :base(hubContext, currentUserProvider)
        {

        }

        public async Task InviteUserAsync(string userUid, int gameId)
        {
            var invition = new Invite(gameId, await _currentUserProvider.GetCurrentUserAsync());
            await _hubContext
                .Clients
                .Group($"{HubGroup.PlayRoom.GetStringValue()}{userUid}")
                .SendAsync(Action.Invocation.GetStringValue(), invition);
        }
    }
}
