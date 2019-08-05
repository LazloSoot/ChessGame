using Chess.Common.Helpers;
using System;
using System.Collections.Generic;

namespace Chess.Common.Interfaces
{
    public interface IChessGame : IEquatable<IChessGame>
    {
        string Fen { get; }
        Color MateTo { get; }
        Color CheckTo { get; }
        bool IsStaleMate { get; }
        IChessGame InitGame(string fen);
        IChessGame InitGame(ChessGameInitSettings initialSettings);
        List<string> GetAllValidMovesForPieceAt(int x, int y);
        char GetPieceAt(int x, int y);
        IChessGame Move(string move);
    }
}
