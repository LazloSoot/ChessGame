using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IGameDataService : ICRUDService<Game, GameFullDTO>
    {
        Task<PagedResultDTO<GamePartialDTO>> GetUserGames(int userID, int? pageIndex, int? pageSize);

        Task<GameFullDTO> CreateNewGameWithFriend(GameFullDTO entity);

        Task<GameFullDTO> CreateNewGameVersusAI(GameFullDTO game);

        Task<GameFullDTO> JoinToGame(int gameId);

        Task<GameFullDTO> ResignGame(int gameId);

        Task<GameFullDTO> SetDraw(int gameId);

        Task<GameFullDTO> SuspendGame(int gameId);
    }
}
