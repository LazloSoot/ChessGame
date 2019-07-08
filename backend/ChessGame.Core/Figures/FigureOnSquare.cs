using Chess.BL.Figures.Helpers;

namespace Chess.BL.Figures
{
    class FigureOnSquare
    {
        public Figure Figure { get; private set; }
        public Square Square { get; private set; }
        public FigureOnSquare(Figure figure, Square square)
        {
            Figure = figure;
            Square = square;
        }
    }
}
