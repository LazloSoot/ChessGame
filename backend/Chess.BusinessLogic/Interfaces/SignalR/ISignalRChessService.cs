using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Chess.BusinessLogic.Interfaces.SignalR
{
    public interface ISignalRChessService
    {

        Task CommitMove(int gameId);
    }
}
