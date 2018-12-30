using Chess.DataAccess.Helpers;
using System.Collections.Generic;

namespace Chess.Common.DTOs
{
    public class GameDTO
    {
        public int Id { get; set; }
        public string Fen { get; set; }
        public GameStatus Status { get; set; }
        public IEnumerable<SideDTO> Sides { get; set; }
        public IEnumerable<MoveDTO> Moves { get; set; }

        public GameDTO()
        {
            this.Sides = new List<SideDTO>();
            this.Moves = new List<MoveDTO>();
        }
    }
}
