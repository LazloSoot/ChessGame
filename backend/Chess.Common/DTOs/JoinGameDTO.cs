using Chess.DataAccess.Helpers;

namespace Chess.Common.DTOs
{
    public class JoinGameDTO
    {
        public int GameId { get; set; }
        public Color SelectedColor { get; set; }
        public UserDTO Player { get; set; }
    }
}
