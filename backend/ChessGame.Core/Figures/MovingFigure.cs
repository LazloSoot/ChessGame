using ChessGame.Core.Figures.Helpers;
using System;

namespace ChessGame.Core.Figures
{
    internal sealed class MovingFigure
    {
        internal Figure Figure { get; private set; }
        internal Square From { get; private set; }
        internal Square To { get; private set; }
        internal Figure Promotion { get; private set; }


        internal int DeltaX { get; private set; }
        internal int DeltaY { get; private set; }
        internal int AbsDeltaX { get; private set; }
        internal int AbsDeltaY { get; private set; }
        internal int SignX { get; private set; }
        internal int SignY { get; private set; }

        internal MovingFigure(FigureOnSquare fs, Square to, Figure promotion = Figure.None)
        {
            Figure = fs.Figure;
            From = fs.Square;
            To = to;
            Promotion = promotion;
            ComputeProps();
        }

        internal MovingFigure(string move) // Pe2e4 Pe7e8Q k0-0-0
        {
            Figure = (Figure)move[0];
            From = new Square(move.Substring(1, 2));
            To = new Square(move.Substring(3, 2));
            Promotion = (move.Length > 5) ? (Figure)move[5] : Figure.None;
            
            ComputeProps();
        }

        private void ComputeProps()
        {
            DeltaX = To.X - From.X;
            DeltaY = To.Y - From.Y;
            AbsDeltaX = Math.Abs(DeltaX);
            AbsDeltaY = Math.Abs(DeltaY);
            SignX = Math.Sign(DeltaX);
            SignY = Math.Sign(DeltaY);
        }
    }
}
