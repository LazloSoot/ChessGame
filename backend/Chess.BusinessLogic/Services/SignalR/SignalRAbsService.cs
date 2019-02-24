using Chess.BusinessLogic.Hubs;
using Chess.BusinessLogic.Interfaces.SignalR;
using Chess.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;

namespace Chess.BusinessLogic.Services.SignalR
{
    [Authorize]
    public abstract class SignalRAbsService<THub> : ISignalRService where THub: CommonHub
    {
        protected readonly IHubContext<THub> _hubContext;
        protected readonly ICurrentUserProvider _currentUserProvider;

        public SignalRAbsService(IHubContext<THub> hubContext, ICurrentUserProvider currentUser)
        {
            _hubContext = hubContext;
            _currentUserProvider = currentUser;
        }

        public Dictionary<string, string> GetOnlineUsersInfo()
        {
            return CommonHub.ConnectedUsers.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public Dictionary<string, string> GetOnlineUsersInfoByNameOrSurnameStartsWith(string part)
        {
            part = part.Trim().ToLower();
            return CommonHub.ConnectedUsers
                .Where(
                        kv => kv.Value
                        .Trim()
                        .ToLower()
                        .Split(' ')
                        .Where(n => n
                            .StartsWith(part))
                            .Count() > 0
                       )
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
