using Chess.DataAccess.Helpers;
namespace Chess.Common.DTOs
{
    public class SideDTO
    {
        public int Id { get; set; }
        public GameDTO Game { get; set; }
        public PlayerDTO Player { get; set; }
        public Color Color { get; set; }
        public int Points { get; set; }
        public bool IsDraw { get; set; }
        public bool IsResign { get; set; }
    }
}
