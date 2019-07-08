using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces.SignalR
{
    public interface ISignalRNotificationService : ISignalRService
    {
        Task InviteUserAsync(string userUid, int gameId);
        Task AcceptInvitation(string inviterUid, int gameId);
    }
}
