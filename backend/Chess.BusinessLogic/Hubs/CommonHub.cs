using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Chess.Common.Helpers;
using Chess.BusinessLogic.Interfaces;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;

namespace Chess.BusinessLogic.Hubs
{
    [Authorize]
    public class CommonHub : Hub
    {
        private IRepository<User> usersProvider;
        protected static ConcurrentDictionary<string, int> ConnectedUsers { get; set; } = new ConcurrentDictionary<string, int>();
     //  public CommonHub(IRepository<User> usersRepo)
     //  {
     //      usersProvider = usersRepo;
     //  }

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
            // NHCblkzx89Qy3xhDgE2GxxkiSqt2
            //int userDbId = await chatService.ChangeUserStatus(targetUserUid: Context.UserIdentifier, isOnline: true);
            var a = Context.UserIdentifier;
            var aa = Context.User.GetName();
            var abc = Context.User.GetUid();
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
