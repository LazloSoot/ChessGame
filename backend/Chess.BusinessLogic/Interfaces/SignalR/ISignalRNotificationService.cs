using Chess.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces.SignalR
{
    public interface ISignalRNotificationService
    {
        Dictionary<string, string> GetOnlineUsersInfoByNameStartsWith(string part);

        Dictionary<string, string> GetOnlineUsersInfo();

        Task InviteUserAsync(string userUid, int gameId);

        Task AcceptInvitation(string inviterUid, int gameId);
    }
}
