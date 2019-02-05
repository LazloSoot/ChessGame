using Chess.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Chess.BusinessLogic.Services.SignalR
{
    [Authorize]
    public abstract class SignalRAbsService<THub> where THub: Hub
    {
        protected readonly IHubContext<THub> _hubContext;
        protected readonly ICurrentUserProvider _currentUserProvider;

        public SignalRAbsService(IHubContext<THub> hubContext, ICurrentUserProvider currentUser)
        {
            _hubContext = hubContext;
            _currentUserProvider = currentUser;
        }
    }
}
