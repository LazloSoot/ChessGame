using Chess.DataAccess.Helpers;
using System;
using System.Collections.Generic;

namespace Chess.Common.DTOs
{
    public class GameFullDTO : DbEntityDTO
    {
        public string Fen { get; set; }
        public GameStatus Status { get; set; }
        public IEnumerable<SideDTO> Sides { get; set; }
        public IEnumerable<MoveDTO> Moves { get; set; }
        public DateTime CreationDate { get; set; }
        public GameFullDTO()
        {
            this.Sides = new List<SideDTO>();
            this.Moves = new List<MoveDTO>();
        }
    }
}
