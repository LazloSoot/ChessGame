using ChessGame.Core.Pieces.Helpers;

namespace ChessGame.Core.Pieces
{
    internal sealed class PieceOnSquare
    {
        /// <summary>
        /// Piece value used in the evaluation of positions.Pawn have the least value, king the bigest.
        /// </summary>
        internal int Value { get; private set; }
        /// <summary>
        /// Sum of attacked pieces values.
        /// </summary>
        internal int AttackedValue { get; set; }
        /// <summary>
        /// Sum of defended pieces values.
        /// </summary>
        internal int DefendedValue { get; set; }
        internal int ValidMovesCount { get; set; }
        internal Piece Piece { get; private set; }
        internal Square Square { get; private set; }
        internal PieceOnSquare(Piece piece, Square square)
        {
            Piece = piece;
            Square = square;
            Value = piece.GetPieceValue();
        }
    }
}
