using Chess.Common.Helpers.ChessGame;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces.SignalR
{
    public interface ISignalRChessService : ISignalRService
    {
        Task CommitMove(int gameId);
        Task EmitMate(int gameId, Color mateTo);
        Task EmitСheck(int gameId, Color checkTo);
        Task EmitResign(int gameId, Color resignedSide);
        Task EmitDraw(int gameId, Color side);
    }
}
