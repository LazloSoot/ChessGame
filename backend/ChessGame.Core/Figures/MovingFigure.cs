﻿using Chess.BL.Figures.Helpers;
using System;

namespace Chess.BL.Figures
{
    class MovingFigure
    {
        public Figure Figure { get; private set; }
        public Square From { get; private set; }
        public Square To { get; private set; }
        public Figure Promotion { get; private set; }


        public int DeltaX { get; private set; }
        public int DeltaY { get; private set; }
        public int AbsDeltaX { get; private set; }
        public int AbsDeltaY { get; private set; }
        public int SignX { get; private set; }
        public int SignY { get; private set; }

        public MovingFigure(FigureOnSquare fs, Square to, Figure promotion = Figure.None)
        {
            Figure = fs.Figure;
            From = fs.Square;
            To = to;
            Promotion = promotion;
            ComputeProps();
        }

        public MovingFigure(string move) // Pe2e4 Pe7e8Q
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