using ChessGame.Core.Figures.Helpers;

namespace ChessGame.Core.Figures
{
    internal sealed class FigureOnSquare
    {
        /// <summary>
        /// Piece value used in the evaluation of positions.Pawn have the least value, king the bigest.
        /// </summary>
        internal int Value { get; private set; }
        /// <summary>
        /// Sum of attacked pieces values.
        /// </summary>
        internal int AttackedValue { get; private set; }
        /// <summary>
        /// Sum of defended pieces values.
        /// </summary>
        internal int DefendedValue { get; private set; }
        /// <summary>
        /// This value should be added/substracted from the score when piece either attacking or defending another chess piece.
        /// This follows the logic that it is better to risk a pawn than it is to risk a queen.
        /// </summary>
        internal int PieceActionValue { get; private set; }
        internal Figure Figure { get; private set; }
        internal Square Square { get; private set; }
        internal FigureOnSquare(Figure figure, Square square)
        {
            Figure = figure;
            Square = square;
            Value = figure.GetPieceValue();
        }
    }
}
