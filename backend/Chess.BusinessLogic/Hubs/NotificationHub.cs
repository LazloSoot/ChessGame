using Chess.DataAccess.Entities;
using Chess.DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Chess.Common.Helpers;
using Chess.Common.Interfaces;

namespace Chess.BusinessLogic.Hubs
{
    [Authorize]
    public class NotificationHub : CommonHub
    {
        public NotificationHub(IRepository<User> usersRepo) 
            : base(usersRepo)
        {

        }
        
        public async Task DismissInvocation(string groupName)
        {
            var currentUserId = Context.UserIdentifier;
            await Clients.Group(groupName).SendCoreAsync(ClientEvent.InvocationDismissed.GetStringValue(), new object[] { currentUserId });
        }

        public async Task CancelInvocation(string groupName)
        {
            var currentUserId = Context.UserIdentifier;
            await Clients.Group(groupName).SendCoreAsync(ClientEvent.InvocationCanceled.GetStringValue(), new object[] { currentUserId });
        }
    }
}
