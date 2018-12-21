using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Hubs
{
    [Authorize]
    public class CommonHub : Hub
    {
        protected static ConcurrentDictionary<string, int> ConnectedUsers { get; set; } = new ConcurrentDictionary<string, int>();

        public virtual async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public virtual async Task LeaveGroup(string groupName)
        {
            try
            {
                // ConnectionId может быть уже недоступен и по истечению time out
                // будет сгенерировано исключение
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            catch (TaskCanceledException ex)
            {

            }
        }

        public override async Task OnConnectedAsync()
        {

            //int userDbId = await chatService.ChangeUserStatus(targetUserUid: Context.UserIdentifier, isOnline: true);
            var a = Context.UserIdentifier;
             // ConnectedUsers.TryAdd(Context.UserIdentifier, userDbId);
             await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await chatService.ChangeUserStatus(targetUserUid: Context.UserIdentifier, isOnline: false);
          //  ConnectedUsers.Remove(Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }

    }
}
