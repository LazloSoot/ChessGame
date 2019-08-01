﻿using System;
using System.Collections.Generic;

namespace ChessGame.Core.Figures.Helpers
{
    struct Square : IEquatable<Square>
    {
        internal int X { get; private set; }
        internal int Y { get; private set; }

        internal Square(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal Square(string squareSymbol)
        {
            if (squareSymbol[0] >= 'a' &&
                squareSymbol[0] <= 'h' &&
                squareSymbol[1] >= '1' &&
                squareSymbol[1] <= '8')
            {
                X = squareSymbol[0] - 'a';
                Y = squareSymbol[1] - '1';
            }
            else
            {
                X = -1;
                Y = -1;
                //    throw new ArgumentOutOfRangeException("Input square symbol is out of board!");
            }
        }

        internal bool IsOnBoard()
        {
            return X > -1 && Y > -1 && X < 8 && Y < 8;
        }

        public bool Equals(Square other)
        {
            return this.X == other.X && this.Y == other.Y;
        }

        public static bool operator ==(Square a, Square b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Square a, Square b)
        {
            return !(a == b);
        }

        internal static IEnumerable<Square> YieldSquares()
        {
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    yield return new Square(x, y);
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Square)
            {
                var other = (Square)obj;
                return this.X == other.X && this.Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() * 17 * Y.GetHashCode();
        }
    }
}
