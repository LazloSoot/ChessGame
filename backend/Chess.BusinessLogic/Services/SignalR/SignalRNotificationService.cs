using Chess.BusinessLogic.Helpers.SignalR;
using Chess.BusinessLogic.Hubs;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.Helpers;
using Chess.Common.Interfaces;
using Chess.DataAccess.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ClientEvent = Chess.Common.Helpers.ClientEvent;

namespace Chess.BusinessLogic.Services.SignalR
{
    public class SignalRNotificationService : SignalRAbsService<NotificationHub>, ISignalRNotificationService
    {
        public SignalRNotificationService(IHubContext<NotificationHub> hubContext, ICurrentUserProvider currentUserProvider)
            :base(hubContext, currentUserProvider)
        {

        }

        public async Task InviteUserAsync(string userUid, int gameId)
        {
            var invition = new Invite(gameId, await _currentUserProvider.GetCurrentUserAsync());
            await _hubContext
                .Clients
                .Group($"{HubGroup.User.GetStringValue()}{userUid}")
                .SendAsync(ClientEvent.Invocation.GetStringValue(), invition);
        }

        public async Task AcceptInvitation(string inviterUid, int gameId)
        {
            await _hubContext
                .Clients
                .Group($"{HubGroup.User.GetStringValue()}{inviterUid}")
                .SendAsync(ClientEvent.InvocationAccepted.GetStringValue(), gameId);
        }
    }
}
