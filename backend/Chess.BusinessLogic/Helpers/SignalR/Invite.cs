using Chess.Common.DTOs;
using Chess.DataAccess.Entities;

namespace Chess.BusinessLogic.Helpers.SignalR
{
    public class Invite
    {
        public int GameId { get; set; }
        public UserDTO Inviter { get; set; }

        public Invite(int gameId, UserDTO inviter)
        {
            this.GameId = gameId;
            this.Inviter = inviter;
        }

        public Invite(int gameId, User inviter)
        {
            this.GameId = gameId;
            this.Inviter = new UserDTO()
            {
                Id = inviter.Id,
                AvatarUrl = inviter.AvatarUrl,
                Name = inviter.Name,
                Uid = inviter.Uid
            };
        }
    }
}
