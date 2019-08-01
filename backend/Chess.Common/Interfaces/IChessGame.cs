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
        IChessGame InitGame(string fen);
        IChessGame InitGame(ChessGameInitSettings initialSettings);
        List<string> GetAllValidMovesForFigureAt(int x, int y);
        char GetFigureAt(int x, int y);
        IChessGame Move(string move);
    }
}
