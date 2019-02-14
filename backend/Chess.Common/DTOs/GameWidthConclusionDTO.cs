using Chess.Common.Helpers;

namespace Chess.Common.DTOs
{
    public sealed class GameWidthConclusionDTO : GameFullDTO
    {
        public DataAccess.Helpers.Color? Side { get; set; }
        public int IsWon { get; set; }
        public bool IsDraw { get; set; }
        public bool IsResigned { get; set; }
    }
}
