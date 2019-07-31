namespace Chess.BL.Moves.Helpers
{
    static class Extentions
    {
        internal static Color FlipColor(this Color color)
        {
            if (color == Color.None)
                return Color.None;

            return color == Color.Black ? Color.White : Color.Black;
        }
    }
}
