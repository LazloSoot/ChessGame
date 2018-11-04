using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IChessMovesService : ICRUDService<Move, CommitedMoveDTO>
    {
        Task<CommitedMoveDTO> Move(MoveDTO moveRequest);

        Task Resign();
    }
}
