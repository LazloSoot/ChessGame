using Chess.BL.Figures.Helpers;

namespace Chess.BL.Figures
{
    internal sealed class FigureOnSquare
    {
        /// <summary>
        /// Piece value used in the evaluation of positions.Pawn have the least value, king the bigest.
        /// </summary>
        internal short Value { get; private set; }
        /// <summary>
        /// Sum of attacked pieces values.
        /// </summary>
        internal short AttackedValue { get; private set; }
        /// <summary>
        /// Sum of defended pieces values.
        /// </summary>
        internal short DefendedValue { get; private set; }
        /// <summary>
        /// Value computed by adding/substracting value when piece either attacking or defending another chess piece.
        /// This follows the logic that it is better to risk a pawn than it is to risk a queen.
        /// </summary>
        internal short PieceImportanceValue { get; private set; }
        internal Figure Figure { get; private set; }
        internal Square Square { get; private set; }
        internal FigureOnSquare(Figure figure, Square square)
        {
            Figure = figure;
            Square = square;
        }
    }
}
