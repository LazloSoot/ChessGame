using Chess.BL.Moves.Helpers;

namespace Chess.BL.Figures.Helpers
{
    static class Extentions
    {
        internal static Color GetColor(this Figure figure)
        {
            if (figure == Figure.None)
                return Color.None;

            return figure.ToString()[0] == 'W' ? Color.White : Color.Black;
        }
    }
}
