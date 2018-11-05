using Chess.DataAccess.Helpers;
namespace Chess.Common.DTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        public string Fen { get; set; }
        public GameStatus Status { get; set; }
    }
}
