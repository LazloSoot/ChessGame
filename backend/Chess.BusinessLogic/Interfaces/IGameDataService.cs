using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IGameDataService : ICRUDService<Game, GameFullDTO>
    {
        Task<IEnumerable<GameWidthConclusionDTO>> GetUserGames(int userID, int? pageIndex, int? pageSize);

        Task<GameFullDTO> CreateNewGameWithFriend(GameFullDTO entity);

        Task<GameFullDTO> CreateNewGameVersusAI(GameFullDTO game);

        Task<GameFullDTO> JoinToGame(int gameId);

        Task<GameFullDTO> SuspendGame(int gameId);
    }
}
