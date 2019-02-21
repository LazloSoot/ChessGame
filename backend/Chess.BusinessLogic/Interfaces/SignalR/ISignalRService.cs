using System.Collections.Generic;

namespace Chess.BusinessLogic.Interfaces.SignalR
{
    public interface ISignalRService
    {
        // uid, userName
        Dictionary<string, string> GetOnlineUsersInfo();
        // uid, userName
        Dictionary<string, string> GetOnlineUsersInfoByNameOrSurnameStartsWith(string part);
    }
}
