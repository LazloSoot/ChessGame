using System;
using System.Collections.Generic;

namespace Chess.Common.Interfaces
{
    public interface IChessGame : IEquatable<IChessGame>
    {
        string Fen { get; }
        IChessGame InitGame(string fen);
        List<string> GetAllValidMovesForFigureAt(int x, int y);
        char GetFigureAt(int x, int y);
        IChessGame Move(string move);
    }
}
