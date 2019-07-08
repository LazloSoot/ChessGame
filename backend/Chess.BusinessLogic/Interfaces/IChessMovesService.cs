using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IChessMovesService : ICRUDService<Move, MoveDTO>
    {
        Task<MoveDTO> Move(MoveRequest move);

        Task<IEnumerable<string>> GetAllValidMovesForFigureAt(int gameId, string squareName);

        Task Resign();
    }
}
