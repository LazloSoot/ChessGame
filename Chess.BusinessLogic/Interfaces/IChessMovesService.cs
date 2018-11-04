using Chess.DataAccess.Entities;
using Chess.Common.DTOs;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IChessMovesService : ICRUDService<Move, MoveDTO>
    {
    }
}
