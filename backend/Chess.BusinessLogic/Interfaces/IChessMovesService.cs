using Chess.DataAccess.Entities;
using Chess.Common.DTOs;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces
{
    public interface IChessMovesService : ICRUDService<Move, MoveDTO>
    {
        Task<MoveDTO> Move(MoveRequest moveRequest);

        Task Resign();
    }
}
