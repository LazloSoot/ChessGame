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

        Task<JoinGameDTO> JoinToGame(JoinGameDTO joinGameData);

        Task<GameDTO> SuspendGame(int gameId);
    }
}
