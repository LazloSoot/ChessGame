using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Chess.Common.Helpers;
using Chess.BusinessLogic.Interfaces;
using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Chess.Common.Interfaces;
using System.Collections.Generic;

namespace Chess.BusinessLogic.Hubs
{
    [Authorize]
    public class CommonHub : Hub
    {
        private readonly IRepository<User> _usersProvider;
        internal static ConcurrentDictionary<string, string> ConnectedUsers { get; private set; } 
        public CommonHub(IRepository<User> usersRepo)
        {
            _usersProvider = usersRepo;
        }

        static CommonHub()
        {
            ConnectedUsers = new ConcurrentDictionary<string, string>(MOCK_USERS);
        }

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

            var uid = Context.UserIdentifier;
            var userName = Context.User.GetName();
            if (string.IsNullOrEmpty(userName))
            {
                userName = (await _usersProvider.GetOneAsync(u => string.Equals(u.Uid, uid))).Name;
            }
             ConnectedUsers.TryAdd(uid, userName);
             await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //await chatService.ChangeUserStatus(targetUserUid: Context.UserIdentifier, isOnline: false);
            ConnectedUsers.TryRemove(Context.UserIdentifier, out string value);
            await base.OnDisconnectedAsync(exception);
        }


        static Dictionary<string, string> MOCK_USERS = new Dictionary<string, string>()
        {
            {"1", "Grisha Petrow"},
            {"2", "Oleg Komarow" },
            {"3", "Marina Popowa Ebat" },
            {"4", "Vladimir Petrogor" },
            {"5", "Anatolii Pomidor" },
            {"6", "pROstoWitia" },
            {"7", "Gena" }
        };
    }
}
