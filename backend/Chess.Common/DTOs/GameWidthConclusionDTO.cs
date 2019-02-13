namespace Chess.Common.DTOs
{
    public sealed class GameWidthConclusionDTO : GameFullDTO
    {
        public int IsWon { get; set; }
        public bool IsDraw { get; set; }
        public bool IsResigned { get; set; }
    }
}
