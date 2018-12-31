using Chess.Common.DTOs;
using Chess.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IGameDataService : ICRUDService<Game, GameDTO>
    {
        Task<GameDTO> CreateNewGame(GameDTO entity);

        Task<GameDTO> JoinToGame(int gameId);

        Task<GameDTO> SuspendGame(int gameId);
    }
}
