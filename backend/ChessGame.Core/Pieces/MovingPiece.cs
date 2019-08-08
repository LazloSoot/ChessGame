using ChessGame.Core.Pieces.Helpers;
using System;

namespace ChessGame.Core.Pieces
{
    internal sealed class MovingPiece : IComparable<MovingPiece>
    {
        internal Piece Piece { get; private set; }
        internal Square From { get; private set; }
        internal Square To { get; private set; }
        internal Piece Promotion { get; private set; }
        /// <summary>
        /// This value should be added/substracted from the score when piece either attacking or defending another chess piece.
        /// This follows the logic that it is better to risk a pawn than it is to risk a queen.
        /// </summary>
        internal int PieceActionValue { get; private set; }
        /// <summary>
        /// Score of current move, initialized by evaluation function
        /// </summary>
        internal int Score { get; set; }

        internal int DeltaX { get; private set; }
        internal int DeltaY { get; private set; }
        internal int AbsDeltaX { get; private set; }
        internal int AbsDeltaY { get; private set; }
        internal int SignX { get; private set; }
        internal int SignY { get; private set; }

        internal MovingPiece(PieceOnSquare fs, Square to, Piece promotion = Piece.None)
        {
            Piece = fs.Piece;
            From = fs.Square;
            To = to;
            Promotion = promotion;
            PieceActionValue = Piece.GetPieceActionValue();
            ComputeProps();
        }

        internal MovingPiece(string move) // Pe2e4 Pe7e8Q k0-0-0
        {
            Piece = (Piece)move[0];
            From = new Square(move.Substring(1, 2));
            To = new Square(move.Substring(3, 2));
            Promotion = (move.Length > 5) ? (Piece)move[5] : Piece.None;
            ComputeProps();
        }

        /// <summary>
        /// Cheks is current move is castling move.
        /// </summary>
        /// <returns></returns>
        internal bool IsItCastlingMove()
        {
            return (Piece == Piece.WhiteKing || Piece == Piece.BlackKing) && (AbsDeltaX == 2 && AbsDeltaY == 0);
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

        public int CompareTo(MovingPiece other)
        {
            return Score.CompareTo(other.Score);
        }
    }
}
